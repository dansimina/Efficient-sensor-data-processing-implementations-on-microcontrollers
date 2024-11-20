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
float M2X[N] = {0};
float varianceX = 0;
float runningSumX = 0;
int ptrX = 0;
int countX = 0;

float valuesY[N] = {0};
float M2Y[N] = {0};
float varianceY = 0;
float runningSumY = 0;
int ptrY = 0;

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

  // computeStandardDeviation("x", angleX, ptrX, countX, runningSumX, valuesX, M2X);

  delay(10);
}

// void computeStandardDeviation(String et, float value, int& ptr, int& count, float& runningSum, float values[50], float M2[50]) {
//   if(count < N) {
//     count++;
//   }

//   int next = (ptr + 1) % N;
//   float oldMean = mean;

//   runningSum = runningSum - values[next] + value;
//   mean = runningSum / count;

  
//   M2[next] = M2[ptr] + (value - oldMean) * (value - mean);
//   float variance = count > 1 ? sqrt(M2[next] / (count - 1)) : 0;

//   values[next] = value;
//   ptr = next;

//   if(count == N) {
//     String ZScore = "zscore" + et;

//     for (int i = (ptr + 1) % N, j = 0; j < N; i = (i + 1) % N, j++) {
//       float zi = 0.0;
//       if(variance > 0) {
//         zi = (values[i] - mean) / variance;
//       }
//       ZScore += " ";
//       ZScore += String(zi);
//     }

//     ZScore += " #";
    
//     Serial.print(ZScore);
//   }
// }

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
