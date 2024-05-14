export const toYesNo = (value: boolean) => value ? "да" : "нет";

export const toReadableUtcDateTime = (value: string) => new Date(value).toLocaleString("ru", {
  year: "numeric",
  month: "long",
  day: "numeric",
  hour: "numeric",
  minute: "numeric",
  second: "numeric",
  timeZone: "UTC"
});

export const toReadableTime = (value: string) => {
  const hhmmss = value.split(":");
  const hours = parseInt(hhmmss[0]);
  const minutes = parseInt(hhmmss[1]);
  const seconds = parseInt(hhmmss[2]);
  return `${hours} ч ${minutes} мин ${seconds} сек`;
}
