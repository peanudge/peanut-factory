/** Formats a Date as "HH:MM:SS" for display purposes. */
export function formatTime(date: Date): string {
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");
  const seconds = String(date.getSeconds()).padStart(2, "0");
  return `${hours}:${minutes}:${seconds}`;
}

/** Formats a Date as "YYYYMMDD_HHmmss_SSS" for use in filenames. */
export function formatFilenameTimestamp(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");
  const seconds = String(date.getSeconds()).padStart(2, "0");
  const millis = String(date.getMilliseconds()).padStart(3, "0");
  return `${year}${month}${day}_${hours}${minutes}${seconds}_${millis}`;
}

/** Formats a Date and sequence index as a capture filename for ZIP export. */
export function formatCaptureFilename(date: Date, index: number): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  const time = formatTime(date).replace(/:/g, "");
  const sequence = String(index).padStart(3, "0");
  return `capture_${year}${month}${day}_${time}_${sequence}.png`;
}
