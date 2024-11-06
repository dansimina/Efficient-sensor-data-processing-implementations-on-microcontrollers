#include <Wire.h>

#define MPU6050_ADDR 0x68  // Adresa I2C a senzorului MPU-6050
#define PWR_MGMT_1 0x6B    // Registrul de putere
#define ACCEL_XOUT_H 0x3B  // Registrul pentru accelerometru (axa X)
#define GYRO_XOUT_H 0x43   // Registrul pentru giroscop (axa X)

unsigned long lastTime;              // Timpul ultimei măsurători
float angleX = 0, angleY = 0;        // Unghiurile calculate
float posX = 0, posY = 0, posZ = 0;  // Poziția față de poziția inițială
float velX = 0, velY = 0, velZ = 0;  // Viteza inițială este zero

const float alpha = 0.96;
const float filterFactor = 0.9;
float prevAccX = 0, prevAccY = 0, prevAccZ = 0;

void setup() {
  Wire.begin();
  Serial.begin(9600);

  // Initializează MPU-6050
  Wire.beginTransmission(MPU6050_ADDR);
  Wire.write(PWR_MGMT_1);  // Selectează registrul de management al puterii
  Wire.write(0);           // Scoate senzorul din modul sleep
  Wire.endTransmission(true);

  lastTime = millis();  // Setează timpul inițial
  Serial.println("MPU6050 initializat!");
}

void loop() {
  int16_t ax, ay, az;
  int16_t gx, gy, gz;

  // Citește valorile accelerometrului și giroscopului
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
  float gyroX = gx / 131.0;  // Pentru MPU-6050: factorul de scalare este 131.0 pentru ±250 dps
  float gyroY = gy / 131.0;
  float gyroZ = gz / 131.0;

  // Calculează timpul scurs între măsurători
  unsigned long currentTime = millis();
  float dt = (currentTime - lastTime) / 1000.0;  // Conversie la secunde
  lastTime = currentTime;

  // Calcularea unghiurilor din giroscop (integrarea datelor de la giroscop)
  angleX += gyroX * dt;
  angleY += gyroY * dt;

  // Integrează accelerația pentru a obține viteza
  velX += accX * dt;
  velY += accY * dt;
  velZ += accZ * dt;

  // Integrează viteza pentru a obține poziția
  posX += velX * dt;
  posY += velY * dt;
  posZ += velZ * dt;

  // Calculează unghiul pe termen lung folosind accelerometrul (roll și pitch)
  float accelAngleX = atan2(accY, accZ) * 180 / PI;
  float accelAngleY = atan2(-accX, sqrt(accY * accY + accZ * accZ)) * 180 / PI;

  angleX = alpha * (angleX) + (1 - alpha) * accelAngleX;
  angleY = alpha * (angleY) + (1 - alpha) * accelAngleY;


  // if (abs(accXComp) < 0.02 && abs(accYComp) < 0.02 && abs(accZComp - 1) < 0.02) {
  //   velX = 0;
  //   velY = 0;
  //   velZ = 0;
  // }
  accX = filterFactor * prevAccX + (1 - filterFactor) * accX;
  accY = filterFactor * prevAccY + (1 - filterFactor) * accY;
  accZ = filterFactor * prevAccZ + (1 - filterFactor) * accZ;
  prevAccX = accX;
  prevAccY = accY;
  prevAccZ = accZ;

  int vel = abs(velX) + abs(velY) + abs(velZ);


  // Afișare unghiuri calculate în Serial Monitor
  // Serial.print("Angle X: ");
  // Serial.print(angleX);
  // Serial.print(" | Angle Y: ");
  // Serial.println(angleY);
  // Afișează poziția față de punctul inițial
  Serial.print("Position X: ");
  Serial.print(posX);
  Serial.print(" | Position Y: ");
  Serial.print(posY);
  Serial.print(" | Position Z: ");
  Serial.println(posZ);
  // Afișează viteza
  Serial.print("Velocity: ");
  Serial.println(vel);

  // Afișare rezultate
  // Serial.print(angleX);
  // Serial.print(" ");
  // Serial.print(angleY);
  // Serial.print(" #");

  delay(50);  // Rată de citire
}

// Funcție pentru a citi valori de la registrii MPU-6050
int16_t readMPU6050(uint8_t reg) {
  Wire.beginTransmission(MPU6050_ADDR);
  Wire.write(reg);
  Wire.endTransmission(false);
  Wire.requestFrom(MPU6050_ADDR, 2, true);

  int16_t value = (Wire.read() << 8) | Wire.read();
  return value;
}
