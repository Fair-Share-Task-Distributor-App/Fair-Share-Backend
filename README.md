# Task Distributor: Fair Share

## Google Play Download Link

Link replace here

---

## Overview

A mobile application that automatically distributes tasks among team members in a fair and balanced way. It ensures equal workload distribution while also considering individual preferences when assigning tasks.

This repository contains the **backend system**, built as a distributed .NET solution using .NET Aspire.

Frontend repository:
https://github.com/Fair-Share-Task-Distributor-App/Fair-Share-Frontend

---

## Architecture

This project uses **.NET Aspire** to orchestrate multiple backend services.

The system is split into multiple services:

- **API Service** = Handles authentication, task and client management, and schedules to service bus
- **Serverless Functions** = Distribute tasks at specific time from service bus
- **Service Bus** = Enables asynchronous communication between services

---

## Project Structure

```plaintext
src/
├── Api/              # ASP.NET MVC
├── Functions/        # Serverless Functions
aspire/
└── AppHost/          # .NET Aspire orchestration layer
```
