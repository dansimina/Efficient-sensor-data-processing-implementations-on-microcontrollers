#include <Wire.h>
#include <Arduino.h>

// MPU6050
#define MPU6050_ADDR 0x68  // Adresa I2C a senzorului MPU-6050
#define PWR_MGMT_1 0x6B    // Registrul de putere
#define ACCEL_XOUT_H 0x3B  // Registrul pentru accelerometru (axa X)
#define GYRO_XOUT_H 0x43   // Registrul pentru giroscop (axa X)
#define NUMBER_OF_VALUES 5

unsigned long lastTime;
float angleX = 0, angleY = 0;
const float alpha = 0.75;

// HC-SR04
#define ECHO_PIN 10
#define TRIG_PIN 9

float distance = 0.0;

// PROGRAM
#define WAIT1 6
#define WAIT2 3
#define N 25
#define PRINT_LEN 15

float valuesX[N] = { 0 };
int nX = 0;

// Welford
int ptrX = N - 1;
float meanX = 0.0;
float M2nX = 0.0;

float valuesY[N] = { 0 };
int nY = 0;

// Welford
int ptrY = N - 1;
float meanY = 0.0;
float M2nY = 0.0;

float valuesD[N] = { 0 };
int nD = 0;

// Welford
int ptrD = N - 1;
float meanD = 0.0;
float M2nD = 0.0;

// Results
float result[N] = { 0 };

// Multitasking
#define NO_OF_TASKS 5
typedef struct {
  void (*taskFunction)();
  unsigned long lastRunTime;
  unsigned long interval;
} Task;

Task task[NO_OF_TASKS];
unsigned long systemTime;

#define BUFFER_SIZE 10
float bufferAngleX[BUFFER_SIZE]{};
int headBufferAngleX = 0;
int tailBufferAngleX = 0;
int sizeBufferAngleX = 0;

float bufferAngleY[BUFFER_SIZE]{};
int headBufferAngleY = 0;
int tailBufferAngleY = 0;
int sizeBufferAngleY = 0;

float bufferDistance[BUFFER_SIZE]{};
int headBufferDistance = 0;
int tailBufferDistance = 0;
int sizeBufferDistance = 0;

void insertIntoBuffer(float value, int& size, int& head, int& tail, float buffer[BUFFER_SIZE]) {
    if(size == BUFFER_SIZE) {
        head = (head + 1) % BUFFER_SIZE;
    }
    size++;
    buffer[tail] = value;
    tail = (tail + 1) % BUFFER_SIZE;
}

float extractFromBuffer(int& size, int& head, int& tail, float buffer[BUFFER_SIZE]) {
    if(size == 0) {
        return buffer[(head - 1 + BUFFER_SIZE) % BUFFER_SIZE];
    }
    size--;
    float value = buffer[head];
    head = (head + 1) % BUFFER_SIZE;

    return value;
}

void task1() {
  readAngles();

  insertIntoBuffer(angleX, sizeBufferAngleX, headBufferAngleX, tailBufferAngleX, bufferAngleX);
  insertIntoBuffer(angleY, sizeBufferAngleY, headBufferAngleY, tailBufferAngleY, bufferAngleY);

  printAngles();
}

void task2() {
  readDistance();

  insertIntoBuffer(distance, sizeBufferDistance, headBufferDistance, tailBufferDistance, bufferDistance);

  printDistance();
}

void task3() {
  float angle = extractFromBuffer(sizeBufferAngleX, headBufferAngleX, tailBufferAngleX, bufferAngleX);

  computeZScoreWelford(angle, ptrX, nX, meanX, valuesX, M2nX, result);
  printZScore("x", result, ptrX);
}

void task4() {
  float angle = extractFromBuffer(sizeBufferAngleY, headBufferAngleY, tailBufferAngleY, bufferAngleY);
  
  computeZScoreWelford(angle, ptrY, nY, meanY, valuesY, M2nY, result);
  printZScore("y", result, ptrY);
}

void task5() {
  float distance = extractFromBuffer(sizeBufferDistance, headBufferDistance, tailBufferDistance, bufferDistance);

  computeZScoreWelford(distance, ptrD, nD, meanD, valuesD, M2nD, result);
  printZScore("d", result, ptrD);
}

void initializeTasks() {
  task[0] = { task1, lastTime, 6 };
  task[1] = { task2, lastTime, 6 };
  task[2] = { task3, lastTime, 8 };
  task[3] = { task4, lastTime, 8 };
  task[4] = { task5, lastTime, 8 };
}

// Time and memory performance measurement
#define TIME_WINDOW 100
int totalRunningTime = 0;
int count = 0;
int maxUsedRAM = 0;

void setup() {
  initializeMPU6050();

  Serial.begin(115200);

  initializeHCSR04();

  lastTime = millis();  // Setează timpul initial

  initializeTasks();
}

void loop() {
  // start time
  unsigned long start = millis();

  // planificator
  systemTime = millis();
  unsigned long longestWaitingPeriod = 0;
  Task* currentTask = nullptr;
  int taskEt = -1;

  for (int i = 0; i < NO_OF_TASKS; i++) {
    unsigned long waitingPeriod = systemTime - task[i].lastRunTime;

    if (waitingPeriod >= task[i].interval && waitingPeriod > longestWaitingPeriod) {
      currentTask = &task[i];
      longestWaitingPeriod = waitingPeriod;
      taskEt = i;

      computeMaxUsedRam();
    }
  }

  if (currentTask != nullptr) {
    currentTask->taskFunction();
    currentTask->lastRunTime = systemTime;
    delay(4);
  }

  //end time
  unsigned long end = millis();

  computeMaxUsedRam();
  computeInfo(start, end);
}

void computeInfo(int start, int end) {
  if(start < end) {
    totalRunningTime += (end - start);
    count++;

    if(count == TIME_WINDOW) {
      float averageTime = (float)totalRunningTime / TIME_WINDOW;

      Serial.print("running_info " + String(averageTime) + " " + String(maxUsedRAM) + " #");
      totalRunningTime = 0;
      count = 0;
      maxUsedRAM = 0;
      delay(WAIT2);  
    }
  }
}

int getUsedRAM() {
    extern int __heap_start, *__brkval;
    int v;
    // Calculate free RAM
    int freeRAM = (int) &v - (__brkval == 0 ? (int) &__heap_start : (int) __brkval);
    // Total RAM on Arduino Uno is 2048 bytes
    return 2048 - freeRAM;
}

void computeMaxUsedRam() {
  maxUsedRAM = max(maxUsedRAM, getUsedRAM());
}

void printZScore(String et, float score[N], int start) {
  String ZScore = "zscore" + et;

  start = (start + N - PRINT_LEN) % N;
  for (int i = 0, j = start; i < PRINT_LEN; i++, j = (j + 1) % N) {
    ZScore += " ";
    ZScore += String(score[j]);
  }

  ZScore += " #";
  Serial.print(ZScore);
  delay(WAIT1);
}

void computeZScoreWelford(float value, int& ptr, int& n, float& mean, float values[N], float& M2, float result[N]) {
  if (n < N) {
    values[ptr] = value;
    n++;
    float delta = value - mean;
    mean += delta / n;
    float delta2 = value - mean;
    M2 += delta * delta2;

    computeMaxUsedRam();
  } else {
    float oldValue = values[ptr];
    values[ptr] = value;

    float deltaOld = oldValue - mean;
    float delta = value - mean;

    mean -= deltaOld / N;
    M2 -= deltaOld * (oldValue - mean);

    mean += delta / N;
    float delta2 = value - mean;
    M2 += delta * delta2;

    computeMaxUsedRam();
  }
  ptr = (ptr + 1) % N;

  float standardDeviation = sqrt(max(M2 / (n - 1), 0.0f));

  for (int i = 0; i < n; i++) {
    if (standardDeviation > 0) {
      result[i] = (values[i] - mean) / standardDeviation;
    } else {
      result[i] = 0;
    }
  }

  computeMaxUsedRam();
}

void printDistance() {
  Serial.print("distance ");
  Serial.print(distance);
  Serial.print(" #");
  delay(WAIT2);
}

void initializeHCSR04() {
  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT);
}

void readDistance() {
  digitalWrite(TRIG_PIN, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG_PIN, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG_PIN, LOW);

  float duration = pulseIn(ECHO_PIN, HIGH);
  // Viteza = Distanta / Timp => Distanta = Viteza * timp, se imparte la doi pentru ca e durata dus intors
  distance = min((duration * 0.0343) / 2, 100.0);
}

void printAngles() {
  float auxAngleX = max(angleX, -89.0);
  auxAngleX = min(angleX, 89.0);
  float auxAngleY = max(angleY, -89.0);
  angleY = min(angleY, 89.0);

  Serial.print("angles ");
  Serial.print(auxAngleX);
  Serial.print(" ");
  Serial.print(auxAngleY);
  Serial.print(" #");
  delay(WAIT2);
}

void initializeMPU6050() {
  Wire.begin();
  // Wake up the MPU6050
  Wire.beginTransmission(MPU6050_ADDR);
  Wire.write(0x6B);  // PWR_MGMT_1 register
  Wire.write(0);     // Set to zero to wake up
  Wire.endTransmission(true);
  
  // Configure accelerometer (+/-2g)
  Wire.beginTransmission(MPU6050_ADDR);
  Wire.write(0x1C);  // ACCEL_CONFIG register
  Wire.write(0x00);  // Set range to +/-2g
  Wire.endTransmission(true);
  
  // Configure gyroscope (+/-250deg/s)
  Wire.beginTransmission(MPU6050_ADDR);
  Wire.write(0x1B);  // GYRO_CONFIG register
  Wire.write(0x00);  // Set range to +/-250deg/s
  Wire.endTransmission(true);
}

void readAngles() {
  float sumAngleX = 0;
  float sumAngleY = 0;
  
  for(int i = 0; i < NUMBER_OF_VALUES; i++) {
    int ax, ay, az;
    int gx, gy, gz;

    // Citeste valorile accelerometrului si giroscopului
    ax = readMPU6050(ACCEL_XOUT_H);
    ay = readMPU6050(ACCEL_XOUT_H + 2);
    az = readMPU6050(ACCEL_XOUT_H + 4);
    gx = readMPU6050(GYRO_XOUT_H);
    gy = readMPU6050(GYRO_XOUT_H + 2);
    gz = readMPU6050(GYRO_XOUT_H + 4);

    // Conversia valorilor brute
    float accX = ax / 16384.0;
    float accY = ay / 16384.0;
    float accZ = az / 16384.0;
    float gyroX = gx / 131.0;
    float gyroY = gy / 131.0;
    float gyroZ = gz / 131.0;

    // Calculeaza timpul scurs intre masuratori
    unsigned long currentTime = millis();
    float dt = (currentTime - lastTime) / 1000.0;  // Conversie la secunde
    lastTime = currentTime;

    // Calcularea unghiurilor din giroscop (integrarea datelor de la giroscop)
    angleX += gyroX * dt;
    angleY += gyroY * dt;

    // Calculeaza unghiul pe termen lung folosind accelerometrul
    float accAngleX = atan2(accY, sqrt(accX * accX + accZ * accZ)) * 180 / PI;
    float accAngleY = atan2(-accX, sqrt(accY * accY + accZ * accZ)) * 180 / PI;

    sumAngleX += alpha * (angleX) + (1 - alpha) * accAngleX;
    sumAngleY += alpha * (angleY) + (1 - alpha) * accAngleY;
  }

  float auxAngleX = sumAngleX / NUMBER_OF_VALUES;
  float auxAngleY = sumAngleY / NUMBER_OF_VALUES;

  if(abs(angleX - auxAngleX) >= 0.5) {
    angleX = auxAngleX;
  }

  if(abs(angleY - auxAngleY) >= 0.5) {
    angleY = auxAngleY;
  }
}

// Functie pentru a citi valori de la registrii MPU-6050
int readMPU6050(unsigned char reg) {
  Wire.beginTransmission(MPU6050_ADDR);
  Wire.write(reg);
  if(Wire.endTransmission(false) != 0) {
    return 0; // Return 0 if transmission fails
  }
  
  if(Wire.requestFrom(MPU6050_ADDR, 2, true) != 2) {
    return 0; // Return 0 if we couldn't get 2 bytes
  }
  
  return (Wire.read() << 8) | Wire.read();
}
