# 🚀 Microcontroller Sensor Signal Processing

This repository contains a project focused on **efficiently processing sensor data** using a **microcontroller (Arduino UNO)**. The implementation is centered around the **Z-Score calculation algorithm**, progressively optimized using **Welford’s method** and **multitasking simulation**. The project culminates in a **performance comparison** of four different implementations in terms of **execution time and memory usage**. A **Windows Forms (.NET) application** is used for **real-time data visualization**.

---

## 📜 Project Overview

### 🔧 Hardware Components
- **Microcontroller**: Arduino UNO (ATmega328P)
- **Sensors**:
  - **MPU-6050** (Accelerometer & Gyroscope)
  - **HC-SR04** (Ultrasonic Distance Sensor)
- **Communication**: Serial interface for data transmission to a **.NET Windows Forms application**.

---

### 📊 Data Processing Techniques
The project progressively **improves the efficiency of sensor data processing** using multiple techniques.

#### Implemented Variants:
1. **Basic Z-Score Computation**:
   - Direct computation of Z-Scores for sensor data.
2. **Optimized Z-Score Computation (Welford’s Algorithm)**:
   - Uses an **online variance calculation** to improve efficiency.
3. **Multitasking Simulation for Basic Z-Score**:
   - Implements a **task scheduler** to handle multiple sensor readings in parallel.
4. **Multitasking Simulation for Welford’s Algorithm**:
   - Uses both **task scheduling and online variance computation**.

---

### 🏗️ System Implementation
Each implementation is tested under **two optimization levels**:
1. **Single-tasking (basic sequential processing).**
2. **Multitasking simulation (task scheduling to improve execution speed).**

The project explores:
- **Execution Time Optimization**:
  - Welford’s method eliminates the need for iterating over all past values.
  - Multitasking simulation reduces bottlenecks by running multiple sensor operations in parallel.
- **Memory Optimization**:
  - Uses **circular buffers** instead of storing entire datasets.
  - Optimized floating-point operations reduce memory overhead.

---

## 📌 Performance Analysis
The project includes detailed **performance benchmarks**, analyzing:
- **Execution time variations** across all four implementations.
- **Memory usage comparison** between different processing methods.
- **Impact of multitasking simulation** on both **time efficiency** and **memory consumption**.

---

## ⚙️ Running the Project

### 1️⃣ Upload the firmware to Arduino:
```sh
arduino-cli compile --fqbn arduino:avr:uno src/
arduino-cli upload -p /dev/ttyUSB0 --fqbn arduino:avr:uno
