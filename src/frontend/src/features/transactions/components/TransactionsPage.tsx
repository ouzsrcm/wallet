import React, { useEffect, useState } from 'react';
import { Card, Space, Button, DatePicker, Select, Input } from 'antd';
import { SearchOutlined, PlusOutlined, FilterOutlined } from '@ant-design/icons';
import TransactionsTable from './TransactionsTable';

// Mock data for demonstration
const mockTransactions = [
  {
    id: 1,
    date: '2023-03-15T10:30:00',
    description: 'Grocery shopping',
    amount: 85.75,
    category: 'Food',
    type: 'expense' as const,
  },
  {
    id: 2,
    date: '2023-03-14T14:45:00',
    description: 'Salary payment',
    amount: 3500.00,
    category: 'Income',
    type: 'income' as const,
  },
  {
    id: 3,
    date: '2023-03-12T09:15:00',
    description: 'Restaurant dinner',
    amount: 125.50,
    category: 'Entertainment',
    type: 'expense' as const,
  },
  {
    id: 4,
    date: '2023-03-10T16:20:00',
    description: 'Freelance work',
    amount: 750.00,
    category: 'Income',
    type: 'income' as const,
  },
  {
    id: 5,
    date: '2023-03-08T11:00:00',
    description: 'Utility bills',
    amount: 210.25,
    category: 'Housing',
    type: 'expense' as const,
  },
];

const TransactionsPage: React.FC = () => {
  const [transactions, setTransactions] = useState(mockTransactions);
  const [loading, setLoading] = useState(false);

  // In a real application, you would fetch data from an API
  useEffect(() => {
    // Simulating API call
    setLoading(true);
    setTimeout(() => {
      setTransactions(mockTransactions);
      setLoading(false);
    }, 500);
  }, []);

  return (
    <div className="transactions-page">
      <h1>Transactions</h1>
      
      <Card>
        <div className="filters-container" style={{ marginBottom: 16 }}>
          <Space wrap>
            <DatePicker.RangePicker placeholder={['Start Date', 'End Date']} />
            
            <Select
              placeholder="Transaction Type"
              style={{ width: 150 }}
              options={[
                { value: 'all', label: 'All Types' },
                { value: 'income', label: 'Income' },
                { value: 'expense', label: 'Expense' },
              ]}
              defaultValue="all"
            />
            
            <Select
              placeholder="Category"
              style={{ width: 150 }}
              options={[
                { value: 'all', label: 'All Categories' },
                { value: 'Food', label: 'Food' },
                { value: 'Housing', label: 'Housing' },
                { value: 'Entertainment', label: 'Entertainment' },
                { value: 'Income', label: 'Income' },
              ]}
              defaultValue="all"
            />
            
            <Input 
              placeholder="Search transactions" 
              prefix={<SearchOutlined />} 
              style={{ width: 200 }}
            />
            
            <Button type="primary" icon={<FilterOutlined />}>
              Apply Filters
            </Button>
          </Space>
        </div>
        
        <div className="actions-container" style={{ marginBottom: 16, display: 'flex', justifyContent: 'flex-end' }}>
          <Button type="primary" icon={<PlusOutlined />}>
            Add Transaction
          </Button>
        </div>
        
        <TransactionsTable transactions={transactions} />
      </Card>
    </div>
  );
};

export default TransactionsPage; 