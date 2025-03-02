import React, { useEffect, useState } from 'react';
import { Button, Table, Modal, Form, Input, Select, message, DatePicker } from 'antd';

import transactionService from '../services/transactionService';
import { TransactionDto } from '../types/transaction';
import { TransactionType } from '../types/enums';
import { PaymentMethod } from '../types/PaymentMethod';
import { enumService } from '../services/enumService';

const TransactionsPage: React.FC = () => {
    const [transactions, setTransactions] = useState<TransactionDto[]>([]);
    const [transactionTypes, setTransactionTypes] = useState<TransactionType[]>([]);
    const [paymentMethods, setPaymentMethods] = useState<PaymentMethod[]>([]);
    const [loading, setLoading] = useState(false);
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [editingTransaction, setEditingTransaction] = useState<TransactionDto | null>(null);

    useEffect(() => {
        fetchTransactions();
        fetchTransactionTypes();
        fetchPaymentMethods();
    }, []);

    const fetchTransactions = async () => {
        try {
            setLoading(true);
            const data = await transactionService.getAll();
            setTransactions(data);
        } catch (error) {
            message.error('Failed to fetch transactions');
        } finally {
            setLoading(false);
        }
    };

    const fetchTransactionTypes = async () => {
        try {
            const data = await enumService.getTransactionTypes();
            setTransactionTypes(data);
        } catch (error) {
            message.error('Failed to fetch transaction types');
        }
    }
    
    const fetchPaymentMethods = async () => {
        try {
            const data = await enumService.getPaymentMethods();
            setPaymentMethods(data);
        } catch (error) {
            message.error('Failed to fetch payment methods');
        }
    }

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
                    { title: 'Transaction Date', dataIndex: 'transactionDate', key: 'transactionDate' },
                    { title: 'Type', dataIndex: 'type', key: 'type' },
                    { title: 'Payment Method', dataIndex: 'paymentMethod', key: 'paymentMethod' },
                    { title: 'Reference', dataIndex: 'reference', key: 'reference' },
                    { title: 'Is Recurring', dataIndex: 'isRecurring', key: 'isRecurring' },
                    //{ title: 'Recurring Period', dataIndex: 'recurringPeriod', key: 'recurringPeriod' },
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
                    <Form.Item name="transactionDate" label="Transaction Date" rules={[{ required: true }]}>
                        <DatePicker />
                    </Form.Item>
                    <Form.Item name="type" label="Type" rules={[{ required: true }]}>
                        <Select>
                            {transactionTypes.map(type => (
                                <Select.Option key={type.id} value={type.id}>
                                    {type.name}
                                </Select.Option>
                            ))}
                        </Select>
                    </Form.Item>
                    <Form.Item name="paymentMethod" label="Payment Method" rules={[{ required: true }]}>
                        <Select>
                            {paymentMethods.map(method => (
                                <Select.Option key={method.id} value={method.id}>
                                    {method.name}
                                </Select.Option>
                            ))}
                        </Select>
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