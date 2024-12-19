#include <Wire.h>
#include <Arduino.h>

// MPU6050
#define MPU6050_ADDR 0x68  // Adresa I2C a senzorului MPU-6050
#define PWR_MGMT_1 0x6B    // Registrul de putere
#define ACCEL_XOUT_H 0x3B  // Registrul pentru accelerometru (axa X)
#define GYRO_XOUT_H 0x43   // Registrul pentru giroscop (axa X)
#define ERR_ANGLE 0.5

unsigned long lastTime;
float angleX = 0, angleY = 0;
float auxAngleX = 0, auxAngleY = 0;
const float alpha = 0.75;

// HC-SR04
#define ECHO_PIN 10
#define TRIG_PIN 9
#define ERR_DISTANCE 1

float distance = 0.0;

// PROGRAM
#define WAIT1 8
#define WAIT2 4
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
}

void loop() {
  // start time
  unsigned long start = millis();

  readAngles();
  printAngles();

  readDistance();
  printDistance();

  computeZScoreWelford(angleX, ptrX, nX, meanX, valuesX, M2nX, result);
  printZScore("x", result, ptrX);

  computeZScoreWelford(angleY, ptrY, nY, meanY, valuesY, M2nY, result);
  printZScore("y", result, ptrY);

  computeZScoreWelford(distance, ptrD, nD, meanD, valuesD, M2nD, result);
  printZScore("d", result, ptrD);

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

  computeMaxUsedRam();
}

void computeZScoreWelford(float value, int& ptr, int& n, float& mean, float values[N], float& M2, float result[N]) {
  if (n < N) {
    values[ptr] = value;
    n++;
    float delta = value - mean;
    mean += delta / n;
    float delta2 = value - mean;
    M2 += delta * delta2;
  } else {
    float oldValue = values[ptr];
    values[ptr] = value;

    float deltaOld = oldValue - mean;
    float deltaNew = value - mean;

    mean -= deltaOld / N;
    M2 -= deltaOld * (oldValue - mean);

    mean += deltaNew / N;
    float delta2 = value - mean;
    M2 += deltaNew * delta2;
  }
  ptr = (ptr + 1) % N;

  float standardDeviation = (n > 1) ? sqrt(max(M2 / (n - 1), 0.0f)) : 0.0f;

  for (int i = 0; i < N; i++) {
    result[i] = (standardDeviation > 0) ? (values[i] - mean) / standardDeviation : 0.0f;
  }

  computeMaxUsedRam();
}

void printDistance() {
  Serial.print("distance ");
  Serial.print(distance);
  Serial.print(" #");
  delay(WAIT2);

  computeMaxUsedRam();
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
  float auxDistance = min((duration * 0.0343) / 2, 100.0);

  if (abs(distance - auxDistance) >= ERR_DISTANCE) {
    distance = auxDistance;
  }

  computeMaxUsedRam();
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

  computeMaxUsedRam();
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
  auxAngleX += gyroX * dt;
  auxAngleY += gyroY * dt;

  // Calculeaza unghiul pe termen lung folosind accelerometrul
  float accAngleX = atan2(accY, sqrt(accX * accX + accZ * accZ)) * 180 / PI;
  float accAngleY = atan2(-accX, sqrt(accY * accY + accZ * accZ)) * 180 / PI;

  auxAngleX = alpha * (auxAngleX) + (1 - alpha) * accAngleX;
  auxAngleY = alpha * (auxAngleY) + (1 - alpha) * accAngleY;

  if (abs(angleX - auxAngleX) >= ERR_ANGLE) {
    angleX = auxAngleX;
  }

  if (abs(angleY - auxAngleY) >= ERR_ANGLE) {
    angleY = auxAngleY;
  }

  computeMaxUsedRam();
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

  computeMaxUsedRam();
  
  return (Wire.read() << 8) | Wire.read();
}
