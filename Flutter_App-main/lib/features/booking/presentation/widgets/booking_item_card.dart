import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:intl/intl.dart';

import '../../../../core/constants/app_colors.dart';
import '../../domain/entities/booking_entity.dart';

class BookingItemCard extends StatelessWidget {
  final BookingEntity booking;
  final VoidCallback? onTap;
  final List<Widget> actions;
  final String? hotelLabel;

  const BookingItemCard({
    super.key,
    required this.booking,
    this.onTap,
    this.actions = const [],
    this.hotelLabel,
  });

  Color get _statusColor {
    switch (booking.status) {
      case 0:
        return AppColors.statusPending;
      case 1:
        return AppColors.statusConfirmed;
      case 2:
        return AppColors.statusCancelled;
      case 3:
        return AppColors.statusCompleted;
      default:
        return AppColors.textMuted;
    }
  }

  @override
  Widget build(BuildContext context) {
    final currencyFmt = NumberFormat.currency(locale: 'vi_VN', symbol: '₫');
    final dateFmt = DateFormat('dd/MM/yyyy');

    return GestureDetector(
      onTap: onTap,
      child: Container(
        margin: const EdgeInsets.only(bottom: 16),
        decoration: BoxDecoration(
          color: AppColors.cardBg,
          borderRadius: BorderRadius.circular(20),
          border: Border.all(color: AppColors.divider.withOpacity(0.6)),
          boxShadow: [
            BoxShadow(
              color: AppColors.textDark.withOpacity(0.04),
              blurRadius: 12,
              offset: const Offset(0, 6),
            ),
          ],
        ),
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.all(16),
              child: Row(
                children: [
                  Container(
                    width: 80,
                    height: 80,
                    decoration: BoxDecoration(
                      color: AppColors.primarySurface,
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: const Icon(
                      Icons.hotel_rounded,
                      color: AppColors.primary,
                      size: 32,
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Text(
                              'Mã #${booking.id}',
                              style: GoogleFonts.dmSans(
                                fontWeight: FontWeight.bold,
                                color: Colors.grey,
                                fontSize: 12,
                              ),
                            ),
                            Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 10,
                                vertical: 4,
                              ),
                              decoration: BoxDecoration(
                                color: _statusColor.withOpacity(0.12),
                                borderRadius: BorderRadius.circular(20),
                                border: Border.all(
                                  color: _statusColor.withOpacity(0.25),
                                  width: 1,
                                ),
                              ),
                              child: Text(
                                booking.statusLabel.toUpperCase(),
                                style: GoogleFonts.dmSans(
                                  color: _statusColor,
                                  fontSize: 9,
                                  fontWeight: FontWeight.bold,
                                  letterSpacing: 0.5,
                                ),
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'Phòng ${booking.roomId}',
                          style: GoogleFonts.dmSans(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        if (hotelLabel != null && hotelLabel!.trim().isNotEmpty) ...[
                          const SizedBox(height: 2),
                          Text(
                            hotelLabel!,
                            style: GoogleFonts.dmSans(
                              fontSize: 13,
                              color: AppColors.primary,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ],
                        const SizedBox(height: 4),
                        Text(
                          '${dateFmt.format(booking.checkInDate)} - ${dateFmt.format(booking.checkOutDate)}',
                          style: GoogleFonts.dmSans(
                            fontSize: 13,
                            color: Colors.grey,
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
            const Divider(height: 1, indent: 16, endIndent: 16),
            Padding(
              padding: const EdgeInsets.all(16),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    '${booking.nightCount} đêm • ${booking.guestCount} khách',
                    style: GoogleFonts.dmSans(
                      color: Colors.grey[600],
                      fontSize: 13,
                    ),
                  ),
                  Text(
                    currencyFmt.format(booking.totalAmount),
                    style: GoogleFonts.dmSans(
                      fontWeight: FontWeight.w800,
                      fontSize: 16,
                      color: AppColors.primary,
                    ),
                  ),
                ],
              ),
            ),
            if (actions.isNotEmpty) ...[
              const Divider(height: 1, indent: 16, endIndent: 16),
              Padding(
                padding: const EdgeInsets.all(16),
                child: Row(
                  children: actions
                      .expand((widget) => [Expanded(child: widget), const SizedBox(width: 12)])
                      .toList()
                    ..removeLast(),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
