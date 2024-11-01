#include <Wire.h>
#include <MPU6050.h>

MPU6050 mpu;

// Variabile pentru date
float accelX, accelY, accelZ;
float angleX, angleY;

void setup() {
  Serial.begin(9600);
  Wire.begin();
  
  // Inițializare MPU6050
  mpu.initialize();
  
  // Verificare conexiune
  if (!mpu.testConnection()) {
    Serial.println("MPU6050 nu este conectat corect!");
    while (1);
  } else {
    Serial.println("MPU6050 conectat cu succes!");
  }
}

void loop() {
  // Citire valori accelerometru
  accelX = mpu.getAccelerationX() / 16384.0; // Conversie pentru a obține valoarea în g
  accelY = mpu.getAccelerationY() / 16384.0;
  accelZ = mpu.getAccelerationZ() / 16384.0;

  // Calculare unghiuri în grade
  angleX = atan2(accelY, sqrt(accelX * accelX + accelZ * accelZ)) * 180 / PI;
  angleY = atan2(-accelX, sqrt(accelY * accelY + accelZ * accelZ)) * 180 / PI;

  // Afișare rezultate
  Serial.print(angleX);
  Serial.print(" ");
  Serial.print(angleY);
  Serial.print(" #");

  delay(100);
}
