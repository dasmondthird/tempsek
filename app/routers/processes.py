# app/routers/processes.py
from fastapi import APIRouter, HTTPException
from typing import List

router = APIRouter(
    prefix="/processes",
    tags=["processes"]
)

@router.get("/", response_model=List[str])
def get_processes():
    # Логика получения процессов
    return ["process1", "process2"]

@router.post("/")
def create_process(process: str):
    # Логика создания процесса
    return {"message": "Процесс создан", "process": process}