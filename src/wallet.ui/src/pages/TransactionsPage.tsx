import React, { useEffect, useState } from 'react';
import { Button, Table, Modal, Form, Input, Select, message } from 'antd';
import transactionService from '../services/transactionService';
import { TransactionDto } from '../types/transaction';

const TransactionsPage: React.FC = () => {
    const [transactions, setTransactions] = useState<TransactionDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [editingTransaction, setEditingTransaction] = useState<TransactionDto | null>(null);

    useEffect(() => {
        fetchTransactions();
    }, []);

    const fetchTransactions = async () => {
        try {
            setLoading(true);
            const data = await transactionService.getAll('user-id'); // Replace 'user-id' with actual user ID
            setTransactions(data);
        } catch (error) {
            message.error('Failed to fetch transactions');
        } finally {
            setLoading(false);
        }
    };

    const handleAdd = () => {
        setEditingTransaction(null);
        setIsModalVisible(true);
    };

    const handleEdit = (transaction: TransactionDto) => {
        setEditingTransaction(transaction);
        setIsModalVisible(true);
    };

    const handleDelete = async (id: string) => {
        try {
            await transactionService.delete(id);
            message.success('Transaction deleted successfully');
            fetchTransactions();
        } catch (error) {
            message.error('Failed to delete transaction');
        }
    };

    const handleOk = async (values: TransactionDto) => {
        try {
            if (editingTransaction) {
                await transactionService.update(editingTransaction.id, values);
                message.success('Transaction updated successfully');
            } else {
                await transactionService.create(values);
                message.success('Transaction added successfully');
            }
            fetchTransactions();
            setIsModalVisible(false);
        } catch (error) {
            message.error('Failed to save transaction');
        }
    };

    return (
        <div>
            <Button type="primary" onClick={handleAdd}>Add Transaction</Button>
            <Table
                rowKey="id"
                dataSource={transactions}
                loading={loading}
                columns={[
                    { title: 'Description', dataIndex: 'description', key: 'description' },
                    { title: 'Amount', dataIndex: 'amount', key: 'amount' },
                    { title: 'Currency', dataIndex: 'currency', key: 'currency' },
                    { title: 'Actions', key: 'actions', render: (_, record) => (
                        <span>
                            <Button onClick={() => handleEdit(record)}>Edit</Button>
                            <Button onClick={() => handleDelete(record.id)}>Delete</Button>
                        </span>
                    )}
                ]}
            />
            <Modal
                title={editingTransaction ? 'Edit Transaction' : 'Add Transaction'}
                open={isModalVisible}
                onCancel={() => setIsModalVisible(false)}
                footer={null}
            >
                <Form
                    initialValues={editingTransaction || { description: '', amount: 0, currency: 'USD' }}
                    onFinish={handleOk}
                >
                    <Form.Item name="description" label="Description" rules={[{ required: true }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item name="amount" label="Amount" rules={[{ required: true }]}>
                        <Input type="number" />
                    </Form.Item>
                    <Form.Item name="currency" label="Currency" rules={[{ required: true }]}>
                        <Select>
                            <Select.Option value="USD">USD</Select.Option>
                            <Select.Option value="EUR">EUR</Select.Option>
                            <Select.Option value="TRY">TRY</Select.Option>
                        </Select>
                    </Form.Item>
                    <Form.Item>
                        <Button type="primary" htmlType="submit">Save</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </div>
    );
};

export default TransactionsPage; 