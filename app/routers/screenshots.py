# app/routers/screenshots.py
from fastapi import APIRouter, HTTPException
from typing import List

router = APIRouter(
    prefix="/screenshots",
    tags=["screenshots"]
)

# Пример модели (обязательно определите соответствующие схемы в `schemas.py`)
# from .. import schemas

@router.get("/", response_model=List[str])
def get_screenshots():
    # Здесь должна быть логика получения скриншотов
    return ["screenshot1.png", "screenshot2.png"]

@router.post("/")
def create_screenshot(screenshot: str):
    # Здесь должна быть логика создания скриншота
    return {"message": "Скриншот создан", "screenshot": screenshot}