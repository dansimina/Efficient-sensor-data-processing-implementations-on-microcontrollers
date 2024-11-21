#include <Wire.h>

// MPU6050
#define MPU6050_ADDR 0x68  // Adresa I2C a senzorului MPU-6050
#define PWR_MGMT_1 0x6B    // Registrul de putere
#define ACCEL_XOUT_H 0x3B  // Registrul pentru accelerometru (axa X)
#define GYRO_XOUT_H 0x43   // Registrul pentru giroscop (axa X)

unsigned long lastTime;
float angleX = 0, angleY = 0;
const float alpha = 0.90;

// HC-SR04
#define ECHO_PIN 10
#define TRIG_PIN 9

float distance = 0.0;

// PROGRAM
#define N 20

float valuesX[N] = {0};
int nX = 0;

// Welford
int ptrX = N - 1;
float meanX = 0.0;
float M2nX = 0.0;

float valuesY[N] = {0};
int nY = 0;

float valuesD[N] = {0};
int nD = 0;

float result[N] = {0};

void setup() {
  Wire.begin();
  Serial.begin(115200);

  // Initializeaza MPU-6050
  Wire.beginTransmission(MPU6050_ADDR);
  Wire.write(PWR_MGMT_1);  // Selecteaza registrul de management al puterii
  Wire.write(0);           // Scoate senzorul din modul sleep
  Wire.endTransmission(true);

  // Seteaz
  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT);

  lastTime = millis();  // Setează timpul initial
}

void loop() {
  readAngles();
  printAngles();

  readDistance();
  printDistance();

  calculateZScore(angleX, nX, valuesX, result);
  printZScore("x", result, 0);

  // calculateZScoreWelford(angleX, ptrX, nX, meanX, valuesX, M2nX, result);
  // printZScore("x", result, ptrX);

  // computeStandardDeviation("x", angleX, ptrX, countX, runningSumX, valuesX, M2X);

  delay(20);
}

void printZScore(String et, float score[N], int start) {
  String ZScore = "zscore" + et;

  for(int i = 0, j = start; i < N; i++, j = (j + 1) % N) {
    ZScore += " ";
    ZScore += String(score[j]);
  }

  // ZScore += " ";
  // ZScore += String(score[N - 1]);

  ZScore += " #";
  Serial.print(ZScore);
}

void calculateZScore(float value, int& n, float values[N], float result[N]) {
  if(n < N) {
    values[n] = value;
    n++;
  }
  else {
    for(int i = 0; i < n - 1; i++) {
      values[i] = values[i + 1];
    }
    values[n - 1] = value;
  }


  float mean = 0.0;
  for(int i = 0; i < n; i++) {
    mean += values[i];
  }
  mean /= n;

  float standardDeviation = 0.0;
  for(int i = 0; i < n; i++) {
    standardDeviation += (values[i] - mean) * (values[i] - mean);
  }

  standardDeviation = sqrt(standardDeviation / (n - 1));

  for(int i = 0; i < n; i++) {
    if(standardDeviation != 0) {
      result[i] = (values[i] - mean) / standardDeviation;
    }
    else {
      result[i] = 0;
    }
  }
}

void calculateZScoreWelford(float value, int& ptr, int& n, float& mean, float values[N], float& M2, float result[N]) {
     if (n < N) {
        values[ptr] = value;
        n++;
        float delta = value - mean;
        mean += delta / n;
        float delta2 = value - mean;
        M2 += delta * delta2;
      } 
      else {
        float oldValue = values[ptr];
        values[ptr] = value;

        float deltaOld = oldValue - mean;
        float delta = value - mean;

        mean -= deltaOld / N;
        M2 -= deltaOld * (oldValue - mean);

        mean += delta / N;
        float delta2 = value - mean;
        M2 += delta * delta2;
      }
      ptr = (ptr + 1) % N;
    
    float standardDeviation = sqrt(max(M2 / (n - 1), 0.0f)); 
    
    for(int i = 0; i < n; i++) {
        if(standardDeviation > 0) {
            result[i] = (values[i] - mean) / standardDeviation;
        } else {
            result[i] = 0;
        }
    }
}

void printDistance() {
  Serial.print("distance ");
  Serial.print(distance);
  Serial.print(" #");
}

void readDistance() {
  digitalWrite(TRIG_PIN, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG_PIN, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG_PIN, LOW);

  float duration = pulseIn(ECHO_PIN, HIGH);
  // Viteza = Distanta / Timp => Distanta = Viteza * timp, se imparte la doi pentru ca e durata dus intors
  distance = min((duration * 0.0343)/2, 100.0);
}

void printAngles() {
  Serial.print("angles ");
  Serial.print(angleX);
  Serial.print(" ");
  Serial.print(angleY);
  Serial.print(" #");
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
  angleX += gyroX * dt;
  angleY += gyroY * dt;

  // Calculeaza unghiul pe termen lung folosind accelerometrul
  float accAngleX = atan2(accY, sqrt(accX * accX + accZ * accZ)) * 180 / PI;
  float accAngleY = atan2(-accX, sqrt(accY * accY + accZ * accZ)) * 180 / PI;

  angleX = alpha * (angleX) + (1 - alpha) * accAngleX;
  angleY = alpha * (angleY) + (1 - alpha) * accAngleY;
  
  angleX = max(angleX, -89.0);
  angleX = min(angleX, 89.0);
  angleY = max(angleY, -89.0);
  angleY = min(angleY, 89.0);
}

// Functie pentru a citi valori de la registrii MPU-6050
int readMPU6050(unsigned char reg) {
  Wire.beginTransmission(MPU6050_ADDR);
  Wire.write(reg);
  Wire.endTransmission(false);
  Wire.requestFrom(MPU6050_ADDR, 2, true);

  int value = (Wire.read() << 8) | Wire.read();
  return value;
}
