import React, { useEffect, useState } from 'react';
import { Button, Table, Modal, Form, Input, Select, message, DatePicker, Checkbox, Row, Col, Divider } from 'antd';

import transactionService from '../services/transactionService';
import { TransactionDto } from '../types/transaction';
import { TransactionType } from '../types/Enums';
import { PaymentMethod } from '../types/PaymentMethod';
import { enumService } from '../services/enumService';
import { CurrencyDto } from '../types/Currency';
import { CategoryDto } from '../types/Category';
import categoryService from '../services/categoryService';
import currencyService from '../services/currencyService';



const TransactionsPage: React.FC = () => {
    const [transactions, setTransactions] = useState<TransactionDto[]>([]);
    const [transactionTypes, setTransactionTypes] = useState<TransactionType[]>([]);
    const [paymentMethods, setPaymentMethods] = useState<PaymentMethod[]>([]);
    const [currencies, setCurrencies] = useState<CurrencyDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [editingTransaction, setEditingTransaction] = useState<TransactionDto | null>(null);
    const [categories, setCategories] = useState<CategoryDto[]>([]);
    const [categoriesLoading, setCategoriesLoading] = useState(false);

    useEffect(() => {
        fetchTransactions();
        fetchTransactionTypes();
        fetchPaymentMethods();
        fetchCategories();
        fetchCurrencies();
    }, []);

    const fetchCurrencies = async () => {
        try {
            setLoading(true);
            const data = await currencyService.getAll();
            setCurrencies(data);
        } catch (error) {
            message.error('Failed to fetch currencies');
        } finally {
            setLoading(false);
        }
    }

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

    const fetchCategories = async () => {
        try {
            setCategoriesLoading(true);
            const data = await categoryService.getAll();
            setCategories(data);
        } catch (error) {
            message.error('Failed to fetch categories');
        } finally {
            setCategoriesLoading(false);
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
            <Divider />
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
                width={800}
            >
                <Form
                    initialValues={editingTransaction || { description: '', amount: 0, currency: 'USD' }}
                    onFinish={handleOk}
                    layout="horizontal"
                    labelCol={{ span: 8 }}
                    wrapperCol={{ span: 16 }}
                >
                    <Row gutter={16}>
                        <Col span={12}>
                            <Form.Item name="description" label="Description" rules={[{ required: true }]}>
                                <Input />
                            </Form.Item>
                        </Col>
                        <Col span={12}>
                            <Form.Item name="amount" label="Amount" rules={[{ required: true }]}>
                                <Input type="number" />
                            </Form.Item>
                        </Col>
                    </Row>

                    <Row gutter={16}>
                        <Col span={12}>
                            <Form.Item name="transactionDate" label="Transaction Date" rules={[{ required: true }]}>
                                <DatePicker style={{ width: '100%' }} />
                            </Form.Item>
                        </Col>
                        <Col span={12}>
                            <Form.Item name="currency" label="Currency" rules={[{ required: true }]}>
                                <Select>
                                    {currencies.map(currency => (
                                        <Select.Option key={currency.id} value={currency.id}>
                                             ({currency.symbol}) {currency.code} {currency.name}
                                        </Select.Option>
                                    ))}
                                </Select>
                            </Form.Item>
                        </Col>
                    </Row>

                    <Row gutter={16}>
                        <Col span={12}>
                            <Form.Item name="type" label="Type" rules={[{ required: true }]}>
                                <Select>
                                    {transactionTypes.map(type => (
                                        <Select.Option key={type.id} value={type.id}>
                                            {type.name}
                                        </Select.Option>
                                    ))}
                                </Select>
                            </Form.Item>
                        </Col>
                        <Col span={12}>
                            <Form.Item name="categoryId" label="Category" rules={[{ required: true }]}>
                                <Select>
                                    {categories.map(category => (
                                        <Select.Option key={category.id} value={category.id}>
                                            {category.name}
                                        </Select.Option>
                                    ))}
                                </Select>
                            </Form.Item>
                        </Col>
                    </Row>

                    <Row gutter={16}>
                        <Col span={12}>
                            <Form.Item name="paymentMethod" label="Payment Method" rules={[{ required: true }]}>
                                <Select>
                                    {paymentMethods.map(method => (
                                        <Select.Option key={method.id} value={method.id}>
                                            {method.name}
                                        </Select.Option>
                                    ))}
                                </Select>
                            </Form.Item>
                        </Col>
                        <Col span={12}>
                            <Form.Item name="reference" label="Reference">
                                <Input />
                                <small>
                                    (Fatura no, sipariş no vs.)
                                </small>
                            </Form.Item>
                        </Col>
                    </Row>

                    <Row gutter={16}>
                        <Col span={12}>
                            <Form.Item name="isRecurring" label="Is Recurring" valuePropName="checked">
                                <Checkbox />
                            </Form.Item>
                        </Col>
                        <Col span={12}>
                            <Form.Item name="recurringPeriod" label="Recurring Period">
                                <Select>
                                    <Select.Option value="Daily" key="Daily">Daily</Select.Option>
                                    <Select.Option value="Weekly" key="Weekly">Weekly</Select.Option>
                                </Select>
                            </Form.Item>
                        </Col>
                    </Row>

                    <Form.Item wrapperCol={{ offset: 4, span: 16 }}>
                        <Button type="primary" htmlType="submit">Save</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </div>
    );
};

export default TransactionsPage; 