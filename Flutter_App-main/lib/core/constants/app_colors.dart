import 'package:flutter/material.dart';

/// Premium Luxury Palette: Deep Slate Navy (Primary) + Champagne Gold (Accent) + Cool Off-white (Background)
class AppColors {
  // ── Primary Slate Navy ──────────────────────────────────────────
  static const Color primary          = Color(0xFF1E3A8A); // Deep Royal Navy
  static const Color primaryMedium    = Color(0xFF2C5282); // Medium Slate Blue
  static const Color primarySurface   = Color(0xFFEFF6FF); // Soft Blue-Grey Tint
  
  // ── Accent Champagne Gold ───────────────────────────────────────
  static const Color accentGold       = Color(0xFFD4AF37); // Luxury Champagne Gold

  // ── Cool Neutral Backgrounds ────────────────────────────────────
  static const Color scaffoldBg       = Color(0xFFF8FAFC); // Cool Slate Off-White
  static const Color cardBg           = Color(0xFFFFFFFF); // Pure White Card Surface
  static const Color divider          = Color(0xFFE2E8F0); // Modern Slate Divider

  // ── Surfaces & Text Details ─────────────────────────────────────
  static const Color textDark         = Color(0xFF0F172A); // Dark Slate Charcoal
  static const Color textMuted        = Color(0xFF64748B); // Medium Slate Grey
  static const Color surfaceLight     = Color(0xFFF1F5F9); // Very Light Grey Surface

  // ── Typography Colors (preserves existing text name mapping) ───
  static const Color textPrimary      = Color(0xFF0F172A); // Dark Slate primary text
  static const Color textSecondary    = Color(0xFF475569); // Medium Slate secondary text
  static const Color textHint         = Color(0xFF94A3B8); // Light Slate hint text

  // ── Semantic Status Colors (strictly green/red for indicators) ──
  static const Color error            = Color(0xFFEF4444); // Modern Red
  static const Color warning          = Color(0xFFF59E0B); // Modern Amber
  static const Color success          = Color(0xFF10B981); // Modern Emerald Green
  static const Color info             = Color(0xFF3B82F6); // Modern Blue

  // ── Booking Status chips ────────────────────────────────────────
  static const Color statusPending    = Color(0xFFF59E0B); // Amber
  static const Color statusConfirmed  = Color(0xFF10B981); // Emerald Green
  static const Color statusCancelled  = Color(0xFFEF4444); // Red
  static const Color statusCompleted  = Color(0xFF3B82F6); // Blue
}
