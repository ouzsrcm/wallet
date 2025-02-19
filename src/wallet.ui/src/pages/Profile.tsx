import React, { useEffect, useState } from 'react';
import { Card, Form, Input, Button, DatePicker, Select, message, Tabs, Space } from 'antd';
import { useSelector } from 'react-redux';
import { RootState } from '../store';
import api from '../services/api';
import dayjs from 'dayjs';
import { personService } from '../services/personService';
import { PersonData } from '../types/person';

const { TabPane } = Tabs;

const Profile = () => {
  const [form] = Form.useForm();
  const { user } = useSelector((state: RootState) => state.auth);
  const [loading, setLoading] = useState(false);
  const [personData, setPersonData] = useState<PersonData | null>(null);

  useEffect(() => {
    fetchPersonData();
  }, []);

  const fetchPersonData = async () => {
    try {
      setLoading(true);
      const data = await personService.getPerson(user?.id);
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

  const handleAddAddress = async (values: any) => {
    try {
      await personService.addAddress(personData?.id!, values);
      message.success('Adres başarıyla eklendi');
      await fetchPersonData();
    } catch (error) {
      message.error('Adres eklenirken hata oluştu');
    }
  };

  const handleUpdateAddress = async (addressId: string, values: any) => {
    try {
      await personService.updateAddress(addressId, values);
      message.success('Adres başarıyla güncellendi');
      await fetchPersonData();
    } catch (error) {
      message.error('Adres güncellenirken hata oluştu');
    }
  };

  const handleDeleteAddress = async (addressId: string) => {
    try {
      await personService.deleteAddress(addressId);
      message.success('Adres başarıyla silindi');
      await fetchPersonData();
    } catch (error) {
      message.error('Adres silinirken hata oluştu');
    }
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
      children: (
        /* Adres listesi ve form buraya gelecek */
        <div>Adresler içeriği</div>
      )
    },
    {
      key: '3',
      label: 'İletişim Bilgileri',
      children: (
        /* İletişim bilgileri listesi ve form buraya gelecek */
        <div>İletişim bilgileri içeriği</div>
      )
    }
  ];

  return (
    <Card title="Profil">
      <Tabs defaultActiveKey="1" items={items} />
    </Card>
  );
};

export default Profile; 