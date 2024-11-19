# app/main.py
from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from .database import engine, Base

app = FastAPI()

# Импорт моделей для создания таблиц
from . import models

Base.metadata.create_all(bind=engine)

# Импорт роутеров
from .routers import diagnostic, video, screenshots, processes

app.include_router(diagnostic.router)
app.include_router(video.router)
app.include_router(screenshots.router)  # Убедитесь, что router существует
app.include_router(processes.router)

# Добавьте корневой маршрут
@app.get("/")
def read_root():
    return {"message": "Добро пожаловать в API!"}

# Импорт менеджера WebSocket
from .websocket.manager import manager

@app.websocket("/ws")
async def websocket_endpoint(websocket: WebSocket):
    await manager.connect(websocket)
    try:
        while True:
            data = await websocket.receive_text()
            # Обработка полученных данных от клиента, если необходимо
            # Например, команды от клиента
    except WebSocketDisconnect:
        manager.disconnect(websocket)
