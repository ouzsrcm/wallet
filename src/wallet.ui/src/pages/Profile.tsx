import React, { useEffect, useState } from 'react';
import { Card, Form, Input, Button, DatePicker, Select, message, Tabs, Space, Table, Modal, Popconfirm, Tag } from 'antd';
import { useSelector } from 'react-redux';
import { RootState } from '../store';
import dayjs from 'dayjs';
import { personService } from '../services/personService';
import { PersonData, PersonAddress } from '../types/person';
import AddressTab from './Profile/AddressTab';
import ContactTab from './Profile/ContactTab';

const { TabPane } = Tabs;

const Profile = () => {
  const [form] = Form.useForm();
  const { user } = useSelector((state: RootState) => state.auth);
  const [loading, setLoading] = useState(false);
  const [personData, setPersonData] = useState<PersonData | null>(null);
  const [addressForm] = Form.useForm();
  const [addressModalVisible, setAddressModalVisible] = useState(false);
  const [editingAddress, setEditingAddress] = useState<PersonAddress | null>(null);
  const [addresses, setAddresses] = useState<PersonAddress[]>([]);
  const [addressLoading, setAddressLoading] = useState(false);

  useEffect(() => {
    fetchPersonData();
  }, [user?.id]);

  const fetchPersonData = async () => {
    try {
      setLoading(true);
      if (!user?.id) {
        message.error('Kullanıcı bilgisi bulunamadı');
        return;
      }
      const data = await personService.getPerson(user.id);
      setPersonData(data);
      form.setFieldsValue({
        ...data,
        dateOfBirth: dayjs(data.dateOfBirth)
      });
    } catch (error) {
      message.error('Kişi bilgileri yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const onFinish = async (values: any) => {
    try {
      setLoading(true);
      await personService.updatePerson(personData?.id!, {
        ...values,
        dateOfBirth: values.dateOfBirth.format('YYYY-MM-DD')
      });
      message.success('Profil başarıyla güncellendi');
      await fetchPersonData();
    } catch (error) {
      message.error('Profil güncellenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleEditAddress = (address: PersonAddress) => {
    setEditingAddress(address);
    addressForm.setFieldsValue(address);
    setAddressModalVisible(true);
  };

  const handleAddContact = async (values: any) => {
    try {
      await personService.addContact(personData?.id!, values);
      message.success('İletişim bilgisi başarıyla eklendi');
      await fetchPersonData();
    } catch (error) {
      message.error('İletişim bilgisi eklenirken hata oluştu');
    }
  };

  const handleUpdateContact = async (contactId: string, values: any) => {
    try {
      await personService.updateContact(contactId, values);
      message.success('İletişim bilgisi başarıyla güncellendi');
      await fetchPersonData();
    } catch (error) {
      message.error('İletişim bilgisi güncellenirken hata oluştu');
    }
  };

  const handleDeleteContact = async (contactId: string) => {
    try {
      await personService.deleteContact(contactId);
      message.success('İletişim bilgisi başarıyla silindi');
      await fetchPersonData();
    } catch (error) {
      message.error('İletişim bilgisi silinirken hata oluştu');
    }
  };

  const items = [
    {
      key: '1',
      label: 'Kişisel Bilgiler',
      children: (
        <Form
          form={form}
          layout="vertical"
          onFinish={onFinish}
          disabled={loading}
        >
          <Form.Item
            name="firstName"
            label="Ad"
            rules={[{ required: true, message: 'Lütfen adınızı girin' }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="lastName"
            label="Soyad"
            rules={[{ required: true, message: 'Lütfen soyadınızı girin' }]}
          >
            <Input />
          </Form.Item>

          <Form.Item name="middleName" label="İkinci Ad">
            <Input />
          </Form.Item>

          <Form.Item
            name="dateOfBirth"
            label="Doğum Tarihi"
            rules={[{ required: true, message: 'Lütfen doğum tarihinizi girin' }]}
          >
            <DatePicker format="DD/MM/YYYY" />
          </Form.Item>

          <Form.Item
            name="gender"
            label="Cinsiyet"
            rules={[{ required: true, message: 'Lütfen cinsiyetinizi seçin' }]}
          >
            <Select>
              <Select.Option value="Male">Erkek</Select.Option>
              <Select.Option value="Female">Kadın</Select.Option>
              <Select.Option value="Other">Diğer</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item name="language" label="Dil">
            <Select>
              <Select.Option value="tr-TR">Türkçe</Select.Option>
              <Select.Option value="en-US">English</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading}>
              Kaydet
            </Button>
          </Form.Item>
        </Form>
      )
    },
    {
      key: '2',
      label: 'Adresler',
      children: <AddressTab personId={personData?.id} />
    },
    {
      key: '3',
      label: 'İletişim Bilgileri',
      children: <ContactTab personId={personData?.id} />
    }
  ];

  return (
    <Card title="Profil">
      <Tabs defaultActiveKey="1" items={items} />
    </Card>
  );
};

export default Profile; 