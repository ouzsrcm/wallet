import React, { useState, useEffect } from 'react';
import { Button, Table, Modal, Form, Input, Space, Popconfirm, Tag } from 'antd';
import { personService } from '../../services/personService';
import { PersonAddress } from '../../types/person';
import { message } from 'antd';
import { 
  PlusOutlined, 
  EditOutlined, 
  DeleteOutlined, 
  StarOutlined,
  HomeOutlined,
  EnvironmentOutlined
} from '@ant-design/icons';

interface AddressTabProps {
  personId?: string;
}

const AddressTab: React.FC<AddressTabProps> = ({ personId }) => {
  const [addressForm] = Form.useForm();
  const [addresses, setAddresses] = useState<PersonAddress[]>([]);
  const [addressLoading, setAddressLoading] = useState(false);
  const [addressModalVisible, setAddressModalVisible] = useState(false);
  const [editingAddress, setEditingAddress] = useState<PersonAddress | null>(null);

  useEffect(() => {
    if (personId) {
      fetchAddresses();
    }
  }, [personId]);

  const fetchAddresses = async () => {
    if (!personId) return;
    try {
      setAddressLoading(true);
      const data = await personService.getAddresses(personId);
      setAddresses(data);
    } catch (error) {
      message.error('Adres bilgileri yüklenirken hata oluştu');
    } finally {
      setAddressLoading(false);
    }
  };

  const handleAddAddress = async (values: Partial<PersonAddress>) => {
    if (!personId) return;
    try {
      setAddressLoading(true);
      await personService.addAddress(personId, values);
      message.success('Adres başarıyla eklendi');
      setAddressModalVisible(false);
      addressForm.resetFields();
      await fetchAddresses();
    } catch (error) {
      message.error('Adres eklenirken hata oluştu');
    } finally {
      setAddressLoading(false);
    }
  };

  const handleEditAddress = (address: PersonAddress) => {
    setEditingAddress(address);
    addressForm.setFieldsValue(address);
    setAddressModalVisible(true);
  };

  const handleUpdateAddress = async (values: Partial<PersonAddress>) => {
    if (!editingAddress?.id) return;
    try {
      setAddressLoading(true);
      await personService.updateAddress(editingAddress.id, values);
      message.success('Adres başarıyla güncellendi');
      setAddressModalVisible(false);
      setEditingAddress(null);
      addressForm.resetFields();
      await fetchAddresses();
    } catch (error) {
      message.error('Adres güncellenirken hata oluştu');
    } finally {
      setAddressLoading(false);
    }
  };

  const handleDeleteAddress = async (addressId: string) => {
    try {
      setAddressLoading(true);
      await personService.deleteAddress(addressId);
      message.success('Adres başarıyla silindi');
      await fetchAddresses();
    } catch (error) {
      message.error('Adres silinirken hata oluştu');
    } finally {
      setAddressLoading(false);
    }
  };

  const handleSetDefaultAddress = async (addressId: string) => {
    try {
      setAddressLoading(true);
      await personService.setDefaultAddress(addressId);
      message.success('Varsayılan adres güncellendi');
      await fetchAddresses();
    } catch (error) {
      message.error('Varsayılan adres güncellenirken hata oluştu');
    } finally {
      setAddressLoading(false);
    }
  };

  const getAddressTypeIcon = (type: string) => {
    switch (type) {
      case 'Home':
        return <HomeOutlined />;
      case 'Work':
        return <EnvironmentOutlined />;
      default:
        return <EnvironmentOutlined />;
    }
  };

  const addressColumns = [
    {
      title: 'Adres Tipi',
      dataIndex: 'addressType',
      key: 'addressType',
      render: (type: string) => (
        <Space>
          {getAddressTypeIcon(type)}
          <span>{type}</span>
        </Space>
      ),
    },
    {
      title: 'Adres Adı',
      dataIndex: 'addressName',
      key: 'addressName',
    },
    {
      title: 'Adres',
      dataIndex: 'addressLine1',
      key: 'addressLine1',
    },
    {
      title: 'Şehir',
      dataIndex: 'city',
      key: 'city',
    },
    {
      title: 'Ülke',
      dataIndex: 'country',
      key: 'country',
    },
    {
      title: 'Varsayılan',
      dataIndex: 'isDefault',
      key: 'isDefault',
      render: (isDefault: boolean) => isDefault ? <Tag color="blue">Varsayılan</Tag> : null,
    },
    {
      title: 'İşlemler',
      key: 'actions',
      render: (_: any, record: PersonAddress) => (
        <Space>
          <Button 
            type="link" 
            icon={<EditOutlined />}
            onClick={() => handleEditAddress(record)}
            title="Düzenle"
          />
          <Popconfirm
            title="Adresi silmek istediğinize emin misiniz?"
            onConfirm={() => handleDeleteAddress(record.id)}
          >
            <Button 
              type="link" 
              danger 
              icon={<DeleteOutlined />}
              title="Sil"
            />
          </Popconfirm>
          {!record.isDefault && (
            <Button 
              type="link"
              icon={<StarOutlined />}
              onClick={() => handleSetDefaultAddress(record.id)}
              title="Varsayılan Yap"
            />
          )}
        </Space>
      ),
    },
  ];

  return (
    <div>
      <Button
        type="primary"
        icon={<PlusOutlined />}
        onClick={() => {
          setEditingAddress(null);
          addressForm.resetFields();
          setAddressModalVisible(true);
        }}
        style={{ marginBottom: 16 }}
      >
        Yeni Adres
      </Button>

      <Table
        columns={addressColumns}
        dataSource={addresses}
        rowKey="id"
        loading={addressLoading}
      />

      <Modal
        title={editingAddress ? 'Adresi Düzenle' : 'Yeni Adres Ekle'}
        open={addressModalVisible}
        onCancel={() => {
          setAddressModalVisible(false);
          setEditingAddress(null);
          addressForm.resetFields();
        }}
        footer={null}
      >
        <Form
          form={addressForm}
          onFinish={editingAddress ? handleUpdateAddress : handleAddAddress}
          layout="vertical"
        >
          <Form.Item
            name="addressName"
            label="Adres Adı"
            rules={[{ required: true, message: 'Lütfen adres adı girin' }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="addressLine1"
            label="Adres"
            rules={[{ required: true, message: 'Lütfen adres girin' }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="city"
            label="Şehir"
            rules={[{ required: true, message: 'Lütfen şehir girin' }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="country"
            label="Ülke"
            rules={[{ required: true, message: 'Lütfen ülke girin' }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="postalCode"
            label="Posta Kodu"
            rules={[{ required: true, message: 'Lütfen posta kodu girin' }]}
          >
            <Input />
          </Form.Item>

          <Form.Item>
            <Space>
              <Button type="primary" htmlType="submit" loading={addressLoading}>
                {editingAddress ? 'Güncelle' : 'Ekle'}
              </Button>
              <Button onClick={() => {
                setAddressModalVisible(false);
                setEditingAddress(null);
                addressForm.resetFields();
              }}>
                İptal
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default AddressTab; 