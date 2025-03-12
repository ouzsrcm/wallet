import React from 'react';
import DataTable, { DataTableColumn } from '../../../shared/components/DataTable';
import { Tag } from 'antd';

// Define the Transaction type
interface Transaction {
  id: string | number;
  date: string;
  description: string;
  amount: number;
  category: string;
  type: 'income' | 'expense';
}

const TransactionsTable: React.FC<{ transactions: Transaction[] }> = ({ transactions }) => {
  // Define columns with date formatting
  const columns: DataTableColumn<Transaction>[] = [
    {
      title: 'Date',
      dataIndex: 'date',
      key: 'date',
      isDate: true, // This enables date formatting
      dateFormat: 'DD MMM YYYY', // Custom format (e.g., "15 Mar 2023")
      sorter: (a, b) => new Date(a.date).getTime() - new Date(b.date).getTime(),
    },
    {
      title: 'Description',
      dataIndex: 'description',
      key: 'description',
    },
    {
      title: 'Category',
      dataIndex: 'category',
      key: 'category',
    },
    {
      title: 'Amount',
      dataIndex: 'amount',
      key: 'amount',
      align: 'right',
      // Custom render function for amount with currency formatting
      render: (amount, record) => {
        const color = record.type === 'income' ? 'green' : 'red';
        const prefix = record.type === 'income' ? '+' : '-';
        return (
          <span style={{ color }}>
            {prefix} ${Math.abs(amount).toFixed(2)}
          </span>
        );
      },
    },
    {
      title: 'Type',
      dataIndex: 'type',
      key: 'type',
      // Custom render function for transaction type
      render: (type) => {
        const color = type === 'income' ? 'success' : 'error';
        return <Tag color={color}>{type.toUpperCase()}</Tag>;
      },
    },
  ];

  return (
    <DataTable
      columns={columns}
      data={transactions}
      pagination={{ pageSize: 10 }}
    />
  );
};

export default TransactionsTable; 