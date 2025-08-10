import {
  DailyOrderStatus,
  ShiftOrderStatus,
  ShiftType,
} from "./services/api.openapi";

export function getEnumValues(
  enumObj: Record<string, string | number>
): { key: string; value: string }[] {
  return Object.keys(enumObj)
    .filter((k: string) => !isNaN(Number(k)))
    .map((key: string) => ({
      key,
      value: String(enumObj[Number(key)]),
    }));
}

export function getDailyOrderStatus(status: DailyOrderStatus) {
  switch (status) {
    case DailyOrderStatus.Ordered:
      return "Ordered";
    case DailyOrderStatus.CanceledBySystem:
      return "Canceled by system";
    case DailyOrderStatus.CanceledByUser:
      return "Canceled by user";
    default:
      return "Unknown";
  }
}

export function getShiftOrderStatus(status: ShiftOrderStatus) {
  switch (status) {
    case ShiftOrderStatus.Ordered:
      return "Ordered";
    case ShiftOrderStatus.Scanned:
      return "Scanned";
    case ShiftOrderStatus.CanceledBySystem:
      return "Canceled by system";
    case ShiftOrderStatus.CanceledByUser:
      return "Canceled by user";
    default:
      return "Unknown";
  }
}

export function getShiftType(shiftType: ShiftType) {
  switch (shiftType) {
    case ShiftType.Shift1:
      return "Shift 1";
    case ShiftType.Shift2:
      return "Shift 2";
    case ShiftType.Shift3:
      return "Shift 3";
    default:
      return "Unknown";
  }
}

export function toShortName(name?: string) {
  if (!name) return "";
  return name
    .split(" ")
    .map((n) => n.charAt(0))
    .join("")
    .toUpperCase();
}
