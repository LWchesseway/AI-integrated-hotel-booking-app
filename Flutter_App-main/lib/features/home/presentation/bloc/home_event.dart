part of 'home_bloc.dart';

abstract class HomeEvent {}

class LoadHomeDataEvent extends HomeEvent {}

class RefreshHotelsEvent extends HomeEvent {
  final ProvinceEntity? province;

  RefreshHotelsEvent({this.province});
}

class LoadMoreHotelsEvent extends HomeEvent {}

class ChangePageEvent extends HomeEvent {
  final int pageIndex;

  ChangePageEvent(this.pageIndex);
}
