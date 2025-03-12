import moment from 'moment';

/**
 * Format a date string or Date object to a readable format
 * @param date - Date string or Date object
 * @param format - Optional format string (defaults to 'DD/MM/YYYY')
 * @returns Formatted date string
 */
export const formatDate = (date: string | Date, format: string = 'DD/MM/YYYY'): string => {
  if (!date) return '';
  return moment(date).format(format);
};

/**
 * Format a date with time
 * @param date - Date string or Date object
 * @param format - Optional format string (defaults to 'DD/MM/YYYY HH:mm')
 * @returns Formatted date and time string
 */
export const formatDateTime = (date: string | Date, format: string = 'DD/MM/YYYY HH:mm'): string => {
  if (!date) return '';
  return moment(date).format(format);
};

/**
 * Get relative time (e.g., "2 hours ago", "yesterday")
 * @param date - Date string or Date object
 * @returns Relative time string
 */
export const getRelativeTime = (date: string | Date): string => {
  if (!date) return '';
  return moment(date).fromNow();
}; 