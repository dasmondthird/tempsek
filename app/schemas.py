# app/schemas.py
from pydantic import BaseModel
from datetime import datetime
from typing import Optional, List

class DiagnosticInfoBase(BaseModel):
    device_id: str
    ip_address: str
    user_name: str
    system_language: str
    antivirus: str
    current_time: datetime

class DiagnosticInfoCreate(DiagnosticInfoBase):
    pass

class DiagnosticInfo(DiagnosticInfoBase):
    id: int
    client_id: int

    class Config:
        from_attributes = True  # Заменено с orm_mode = True

# Пример для других моделей
class VideoFrameBase(BaseModel):
    frame_data: bytes
    client_id: int

class VideoFrameCreate(VideoFrameBase):
    pass

class VideoFrame(VideoFrameBase):
    id: int

    class Config:
        from_attributes = True

class ScreenshotBase(BaseModel):
    image_data: bytes
    client_id: int

class ScreenshotCreate(ScreenshotBase):
    pass

class Screenshot(ScreenshotBase):
    id: int

    class Config:
        from_attributes = True

class ProcessInfoBase(BaseModel):
    process_name: str
    client_id: int

class ProcessInfoCreate(ProcessInfoBase):
    pass

class ProcessInfo(ProcessInfoBase):
    id: int

    class Config:
        from_attributes = True

# Обновите модель пользователя
class UserCreate(BaseModel):
    username: str
    password: str

class User(BaseModel):
    id: int
    username: str

    class Config:
        from_attributes = True  # Добавлено

class Token(BaseModel):
    access_token: str
    token_type: str

class TokenData(BaseModel):
    username: Optional[str] = None