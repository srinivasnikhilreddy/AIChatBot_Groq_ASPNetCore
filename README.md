# AI Chatbot Web Application

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Overview

**AI Chatbot** is a real-time web application built with **ASP.NET Core**, **Entity Framework Core**, and the **Groq API (LLaMA 3.1-8B)**. The chatbot provides interactive AI-powered conversations, streaming responses token-by-token for an immersive experience. All chat history is stored in **SQL Server** for context-aware conversations.

---

## Features

- **Real-Time Streaming AI Responses**  
- **Persistent Chat History** with SQL Server  
- **Markdown Support** for rich text formatting  
- **Responsive Dark-Themed UI** using HTML, CSS, and JavaScript  
- **Context-Aware Replies** using last 10 messages  

---

## Tech Stack

- **Backend**: ASP.NET Core 7, C#  
- **Database**: SQL Server, Entity Framework Core  
- **Frontend**: Razor Pages, HTML, CSS, JavaScript  
- **AI Integration**: Groq API (`llama-3.1-8b-instant`)  
- **Markdown Rendering**: [marked.js](https://marked.js.org/)  

---

## Architecture
![Image](https://github.com/user-attachments/assets/a2f19a52-cefb-45cd-a208-2cf3a2c7d766)
- **Frontend**: Handles user input, renders streamed AI responses dynamically.  
- **Controller**: Orchestrates requests to Groq API, manages streaming, saves chat history.  
- **Database**: Stores chat history with roles, timestamps, and content for future analysis or personalization.

---

## Setup
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "GroqSettings": {
    "apiKey": "YOUR_GROQ_API_KEY_HERE"
  },
  "ConnectionStrings": {
    "sqlconstr": "YOUR_DATABASE_URL_HERE"
  }
}

---

## Implementation Snapshots
<img width="1366" height="768" alt="Image" src="https://github.com/user-attachments/assets/2caf66a4-650d-430e-97f1-5d33fd6aa115" />


<img width="1366" height="768" alt="Image" src="https://github.com/user-attachments/assets/4f1c8e8b-dd36-497b-87b2-6d91f4ac1e3d" />


<img width="1366" height="768" alt="Image" src="https://github.com/user-attachments/assets/4176af8b-bc0a-47ab-97db-6d39398380dd" />

---

## Future Use Cases

**1. Enterprise Customer Support**
This is a natural evolution of chatbots. Integrating with CRMs makes the bot not just reactive but context-aware, which is where enterprise chatbots are heading.

**2. Knowledge Base Assistant**
Expands the chatbot’s role from just answering queries to being an intelligent internal assistant, improving efficiency and employee productivity.

**3. Personalized AI Companion**
Personalization and memory are key trends in AI. Chatbots that remember user preferences and provide tailored interactions are considered next-gen.

**4. Analytics & Insights**
Most future chatbots won’t just converse; they will generate actionable business intelligence from interactions, making this a crucial enhancement.

**5. Multi-Language Support**
Global scalability is a must for future-ready chatbots. Supporting multiple languages ensures broader adoption and inclusivity.
