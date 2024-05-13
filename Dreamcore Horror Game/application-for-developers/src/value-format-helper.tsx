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
