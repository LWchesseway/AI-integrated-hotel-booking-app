import 'package:flutter/material.dart';

class FadeIndexedStack extends StatelessWidget {
  final int index;
  final List<Widget> children;
  final Duration duration;

  const FadeIndexedStack({
    super.key,
    required this.index,
    required this.children,
    this.duration = const Duration(milliseconds: 250),
  });

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: List.generate(children.length, (i) {
        final isActive = i == index;
        return AnimatedOpacity(
          opacity: isActive ? 1.0 : 0.0,
          duration: duration,
          curve: Curves.easeInOut,
          child: IgnorePointer(
            ignoring: !isActive,
            child: children[i],
          ),
        );
      }),
    );
  }
}
