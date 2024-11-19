# app/models.py
from sqlalchemy import Column, Integer, String, DateTime, ForeignKey
from sqlalchemy.orm import relationship
from .database import Base

class Client(Base):
    __tablename__ = "clients"
    id = Column(Integer, primary_key=True, index=True)
    antivirus = Column(String, nullable=True)
    created_at = Column(DateTime, nullable=False)
    last_seen = Column(DateTime, nullable=False)

    # Отношения
    diagnostics = relationship("DiagnosticInfo", back_populates="client")
    video_frames = relationship("VideoFrame", back_populates="client")
    screenshots = relationship("Screenshot", back_populates="client")
    processes = relationship("ProcessInfo", back_populates="client")

class DiagnosticInfo(Base):
    __tablename__ = "diagnostic_info"

    id = Column(Integer, primary_key=True, index=True)
    client_id = Column(Integer, ForeignKey("clients.id"))
    ip_address = Column(String, nullable=True)
    user_name = Column(String, nullable=True)
    system_language = Column(String, nullable=True)
    antivirus = Column(String, nullable=True)
    device_id = Column(String, nullable=True)
    current_time = Column(DateTime, nullable=False)

    client = relationship("Client", back_populates="diagnostics")

# Аналогично создайте модели VideoFrame, Screenshot, ProcessInfo

class User(Base):
    __tablename__ = "users"
    id = Column(Integer, primary_key=True, index=True)
    username = Column(String, unique=True, index=True, nullable=False)
    hashed_password = Column(String, nullable=False)
