import React from 'react';
import { Table } from 'antd';
import type { TableProps } from 'antd';
import { formatDate, formatDateTime } from '../../utils/dateUtils';

// Define column types that include date formatting options
export interface DataTableColumn<T> extends Omit<TableProps<T>['columns'][0], 'render'> {
  dataIndex: string;
  key: string;
  isDate?: boolean;
  isDateTime?: boolean;
  dateFormat?: string;
  render?: (value: any, record: T, index: number) => React.ReactNode;
}

interface DataTableProps<T> extends Omit<TableProps<T>, 'columns'> {
  columns: DataTableColumn<T>[];
  data: T[];
}

/**
 * A reusable data table component with built-in date formatting
 */
const DataTable = <T extends object>({ columns, data, ...rest }: DataTableProps<T>) => {
  // Process columns to add date formatting
  const processedColumns = columns.map(column => {
    // If column already has a custom render function, use that
    if (column.render) {
      return column;
    }
    
    // Add date formatting for date columns
    if (column.isDate) {
      return {
        ...column,
        render: (value: any) => formatDate(value, column.dateFormat)
      };
    }
    
    // Add date-time formatting for datetime columns
    if (column.isDateTime) {
      return {
        ...column,
        render: (value: any) => formatDateTime(value, column.dateFormat)
      };
    }
    
    // Return the column unchanged if it's not a date column
    return column;
  });

  return (
    <Table
      columns={processedColumns}
      dataSource={data}
      rowKey="id"
      {...rest}
    />
  );
};

export default DataTable; 