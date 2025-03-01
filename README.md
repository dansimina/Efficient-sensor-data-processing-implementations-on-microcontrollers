# 🚀 Microcontroller Sensor Signal Processing

This repository contains a project focused on **efficiently processing sensor data** using a **microcontroller (Arduino UNO)**. The implementation optimizes sensor data processing through **real-time filtering, multitasking, and statistical algorithms** to improve performance in **embedded systems**. A **Windows Forms (.NET) application** is used for **data visualization**.

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
The project **acquires, processes, and optimizes** data from sensors using **various algorithms**.

#### Implemented Features:
- **Real-time sensor data acquisition** from MPU-6050 and HC-SR04.
- **Signal filtering**:
  - Moving Average Filter
  - Online Variance Computation (Welford’s Algorithm)
- **Statistical Processing**:
  - Z-Score normalization for motion analysis.
- **Task Scheduling & Multitasking**:
  - Implements an **event-driven task scheduler** to handle multiple sensor readings efficiently.
- **Data Transmission & Visualization**:
  - Sends processed data to a **Windows Forms (.NET) application** via serial communication.
  - Real-time display of **sensor readings, filtering results, and computed statistics**.

---

### 🏗️ System Implementation
- **Single-tasking & Multitasking Approaches**:
  - Initial implementation focused purely on functionality.
  - Optimized implementation with multitasking and resource management.
- **Execution Time & Memory Optimization**:
  - Performance evaluation for **single-tasking vs. multitasking** methods.
  - Comparison of **Welford’s Algorithm vs. standard moving average** for efficiency.
