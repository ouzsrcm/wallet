import React, { useEffect, useState } from 'react';
import { Button, Modal, Form, Input, message, Tree, Spin, Select, Dropdown, Menu, ColorPicker } from 'antd';
import type { Color } from 'antd/es/color-picker';
import categoryService from '../services/categoryService';
import { CategoryDto } from '../types/category';
import './CategoriesPage.css';
import { enumService } from '../services/enumService';
import { TransactionType } from '../types/enums';

const CategoriesPage: React.FC = () => {
    const [categories, setCategories] = useState<CategoryDto[]>([]);
    const [transactionTypes, setTransactionTypes] = useState<TransactionType[]>([]);
    const [loading, setLoading] = useState(false);
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [editingCategory, setEditingCategory] = useState<CategoryDto | null>(null);

    useEffect(() => {
        fetchTransActionTypes();
    }, []);

    const fetchTransActionTypes = async () => {
        try {
            setLoading(true);
            const data = await enumService.getTransactionTypes();
            setTransactionTypes(data);
        } catch (error) {
            message.error('Failed to fetch categories');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCategories();
    }, []);

    const fetchCategories = async () => {
        try {
            setLoading(true);
            const data = await categoryService.getAll();
            setCategories(data);
        } catch (error) {
            message.error('Failed to fetch categories');
        } finally {
            setLoading(false);
        }
    };

    const handleAdd = () => {
        setEditingCategory(null);
        setIsModalVisible(true);
    };

    const handleEdit = (category: CategoryDto) => {
        console.log(category);
        setEditingCategory(category);
        setIsModalVisible(true);
    };

    const handleDelete = async (id: string) => {
        try {
            await categoryService.delete(id);
            message.success('Category deleted successfully');
            fetchCategories();
        } catch (error) {
            message.error('Failed to delete category');
        }
    };

    const handleOk = async (values: any) => {
        try {
            const formData = {
                ...values,
                color: typeof values.color === 'string' 
                    ? values.color 
                    : values.color?.toHexString()
            };

            if (editingCategory) {
                await categoryService.update(editingCategory.id, formData);
                message.success('Category updated successfully');
            } else {
                await categoryService.create(formData);
                message.success('Category added successfully');
            }
            fetchCategories();
            setIsModalVisible(false);
        } catch (error) {
            message.error('Failed to save category');
        }
    };

    const buildTreeData = (categories: CategoryDto[]) => {
        const map = new Map<string, any>();
        const roots: any[] = [];

        categories.forEach(category => {
            map.set(category.id, { ...category, key: category.id, children: [] });
        });

        categories.forEach(category => {
            if (category.parentCategoryId) {
                const parent = map.get(category.parentCategoryId);
                if (parent) {
                    parent.children.push(map.get(category.id));
                }
            } else {
                roots.push(map.get(category.id));
            }
        });

        return roots;
    };

    const menu = (nodeData: any) => [
        {
            key: 'edit',
            label: 'Edit',
            onClick: () => handleEdit(nodeData)
        },
        {
            key: 'delete',
            label: 'Delete',
            onClick: () => handleDelete(nodeData.id)
        }
    ];

    return (
        <div>
            <Button type="primary" onClick={handleAdd}>Add Category</Button>
            {loading ? (
                <Spin />
            ) : (
                <Tree
                    className="category-tree"
                    treeData={buildTreeData(categories)}
                    titleRender={(nodeData) => (
                        <span className="category-node">
                            <Dropdown menu={{ items: menu(nodeData) }} trigger={['click']}>
                                <Button size="small">Actions</Button>
                            </Dropdown>
                            <span style={{ marginLeft: 20 }}>
                                {nodeData.name}
                                {nodeData.color && (
                                    <span 
                                        className="color-preview" 
                                        style={{ 
                                            display: 'inline-block',
                                            width: '16px',
                                            height: '16px',
                                            borderRadius: '4px',
                                            backgroundColor: nodeData.color,
                                            marginLeft: '8px',
                                            verticalAlign: 'middle'
                                        }}
                                    />
                                )}
                            </span>
                        </span>
                    )}
                />
            )}
            <Modal
                title={editingCategory ? 'Edit Category' : 'Add Category'}
                open={isModalVisible}
                onCancel={() => setIsModalVisible(false)}
                footer={null}
            >
                <Form
                    initialValues={{
                        name: editingCategory?.name || '',
                        description: editingCategory?.description || '',
                        parentCategoryId: editingCategory?.parentCategoryId || null,
                        type: editingCategory?.type || '',
                        color: editingCategory?.color || '#1890ff',
                        icon: editingCategory?.icon || ''
                    }}
                    onFinish={handleOk}
                >
                    <Form.Item name="name" label="Name" rules={[{ required: true }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item name="description" label="Description">
                        <Input />
                    </Form.Item>
                    <Form.Item name="icon" label="Icon" rules={[{ required: true }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item name="color" label="Color" rules={[{ required: true }]}>
                        <ColorPicker 
                            format="hex" 
                            showText={(color) => color.toHexString()}
                        />
                    </Form.Item>
                    <Form.Item name="type" label="Type" rules={[{ required: true }]}>
                        <Select>
                            {transactionTypes.map((x) => (
                                <Select.Option key={x.id} value={x.id}>{x.title} ({x.name})</Select.Option>
                            ))}
                        </Select>
                    </Form.Item>
                    <Form.Item name="parentCategoryId" label="Parent Category">
                        <Select allowClear>
                            <Select.Option value={null}>None</Select.Option>
                            {categories.filter(x => x.parentCategoryId === null).map(category => (
                                <Select.Option key={category.id} value={category.id}>
                                    {category.name}
                                </Select.Option>
                            ))}
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

export default CategoriesPage; 