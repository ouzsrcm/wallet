# Utility Functions

This directory contains utility functions that can be used across the application.

## Date Utilities

The `dateUtils.ts` file provides functions for formatting dates:

### `formatDate(date, format)`

Formats a date to a readable format.

- `date`: Date string or Date object
- `format`: Optional format string (defaults to 'DD/MM/YYYY')

Example:
```typescript
import { formatDate } from '../utils/dateUtils';

// Format a date
const formattedDate = formatDate('2023-03-15'); // Returns "15/03/2023"

// Format with custom format
const customFormat = formatDate('2023-03-15', 'MMMM DD, YYYY'); // Returns "March 15, 2023"
```

### `formatDateTime(date, format)`

Formats a date with time.

- `date`: Date string or Date object
- `format`: Optional format string (defaults to 'DD/MM/YYYY HH:mm')

Example:
```typescript
import { formatDateTime } from '../utils/dateUtils';

// Format a date with time
const formattedDateTime = formatDateTime('2023-03-15T14:30:00'); // Returns "15/03/2023 14:30"
```

### `getRelativeTime(date)`

Gets relative time (e.g., "2 hours ago", "yesterday").

- `date`: Date string or Date object

Example:
```typescript
import { getRelativeTime } from '../utils/dateUtils';

// Get relative time
const relativeTime = getRelativeTime('2023-03-15T14:30:00'); // Returns something like "2 days ago"
```

## Date Formatting in Tables

To format dates in tables, use the `DataTable` component from `src/shared/components/DataTable.tsx`:

```typescript
import DataTable, { DataTableColumn } from '../shared/components/DataTable';

// Define columns with date formatting
const columns: DataTableColumn<YourDataType>[] = [
  {
    title: 'Date',
    dataIndex: 'date',
    key: 'date',
    isDate: true, // Enable date formatting
    dateFormat: 'DD MMM YYYY', // Optional custom format
  },
  {
    title: 'Created At',
    dataIndex: 'createdAt',
    key: 'createdAt',
    isDateTime: true, // Enable date-time formatting
  },
  // Other columns...
];

// Use the DataTable component
<DataTable columns={columns} data={yourData} />
```

## Moment.js Format Tokens

The date formatting uses Moment.js. Here are some common format tokens:

- `YYYY`: 4-digit year (e.g., 2023)
- `MM`: 2-digit month (01-12)
- `DD`: 2-digit day (01-31)
- `MMM`: 3-letter month name (e.g., Jan)
- `MMMM`: Full month name (e.g., January)
- `HH`: 2-digit hour in 24-hour format (00-23)
- `hh`: 2-digit hour in 12-hour format (01-12)
- `mm`: 2-digit minute (00-59)
- `ss`: 2-digit second (00-59)
- `A`: AM/PM

For more format tokens, refer to the [Moment.js documentation](https://momentjs.com/docs/#/displaying/format/). 