import React, { useState, useEffect } from 'react';
import { Button, Table, Modal, Form, Input, Space, Popconfirm, Tag, Select } from 'antd';
import { 
  PlusOutlined, 
  EditOutlined, 
  DeleteOutlined, 
  StarOutlined,
  PhoneOutlined,
  MailOutlined,
  MobileOutlined,
  PhoneFilled
} from '@ant-design/icons';
import { personService } from '../../services/personService';
import { PersonContact } from '../../types/person';
import { message } from 'antd';

interface ContactTabProps {
  personId?: string;
}

const ContactTab: React.FC<ContactTabProps> = ({ personId }) => {
  const [contactForm] = Form.useForm();
  const [contacts, setContacts] = useState<PersonContact[]>([]);
  const [contactLoading, setContactLoading] = useState(false);
  const [contactModalVisible, setContactModalVisible] = useState(false);
  const [editingContact, setEditingContact] = useState<PersonContact | null>(null);

  useEffect(() => {
    if (personId) {
      fetchContacts();
    }
  }, [personId]);

  const fetchContacts = async () => {
    if (!personId) return;
    try {
      setContactLoading(true);
      const data = await personService.getContacts(personId);
      setContacts(data);
    } catch (error) {
      message.error('İletişim bilgileri yüklenirken hata oluştu');
    } finally {
      setContactLoading(false);
    }
  };

  const handleAddContact = async (values: Partial<PersonContact>) => {
    if (!personId) return;
    try {
      setContactLoading(true);
      await personService.addContact(personId, values);
      message.success('İletişim bilgisi başarıyla eklendi');
      setContactModalVisible(false);
      contactForm.resetFields();
      await fetchContacts();
    } catch (error) {
      message.error('İletişim bilgisi eklenirken hata oluştu');
    } finally {
      setContactLoading(false);
    }
  };

  const handleEditContact = (contact: PersonContact) => {
    setEditingContact(contact);
    contactForm.setFieldsValue(contact);
    setContactModalVisible(true);
  };

  const handleUpdateContact = async (values: Partial<PersonContact>) => {
    if (!editingContact?.id) return;
    try {
      setContactLoading(true);
      await personService.updateContact(editingContact.id, values);
      message.success('İletişim bilgisi başarıyla güncellendi');
      setContactModalVisible(false);
      setEditingContact(null);
      contactForm.resetFields();
      await fetchContacts();
    } catch (error) {
      message.error('İletişim bilgisi güncellenirken hata oluştu');
    } finally {
      setContactLoading(false);
    }
  };

  const handleDeleteContact = async (contactId: string) => {
    try {
      setContactLoading(true);
      await personService.deleteContact(contactId);
      message.success('İletişim bilgisi başarıyla silindi');
      await fetchContacts();
    } catch (error) {
      message.error('İletişim bilgisi silinirken hata oluştu');
    } finally {
      setContactLoading(false);
    }
  };

  const handleSetDefaultContact = async (contactId: string) => {
    try {
      setContactLoading(true);
      await personService.setDefaultContact(contactId);
      message.success('Varsayılan iletişim bilgisi güncellendi');
      await fetchContacts();
    } catch (error) {
      message.error('Varsayılan iletişim bilgisi güncellenirken hata oluştu');
    } finally {
      setContactLoading(false);
    }
  };

  const getContactTypeIcon = (type: string) => {
    switch (type) {
      case 'Email':
        return <MailOutlined />;
      case 'Phone':
        return <PhoneOutlined />;
      case 'Mobile':
        return <MobileOutlined />;
      case 'Work':
        return <PhoneFilled />;
      default:
        return null;
    }
  };

  const contactColumns = [
    {
      title: 'İletişim Tipi',
      dataIndex: 'contactType',
      key: 'contactType',
      render: (type: string) => (
        <Space>
          {getContactTypeIcon(type)}
          <span>{type}</span>
        </Space>
      ),
    },
    {
      title: 'İletişim Adı',
      dataIndex: 'contactName',
      key: 'contactName',
    },
    {
      title: 'İletişim Değeri',
      dataIndex: 'contactValue',
      key: 'contactValue',
    },
    {
      title: 'Varsayılan',
      dataIndex: 'isDefault',
      key: 'isDefault',
      render: (isDefault: boolean) => isDefault ? <Tag color="blue">Varsayılan</Tag> : null,
    },
    {
      title: 'Birincil',
      dataIndex: 'isPrimary',
      key: 'isPrimary',
      render: (isPrimary: boolean) => isPrimary ? <Tag color="green">Birincil</Tag> : null,
    },
    {
      title: 'İşlemler',
      key: 'actions',
      render: (_: any, record: PersonContact) => (
        <Space>
          <Button 
            type="link" 
            icon={<EditOutlined />}
            onClick={() => handleEditContact(record)}
            title="Düzenle"
          />
          <Popconfirm
            title="İletişim bilgisini silmek istediğinize emin misiniz?"
            onConfirm={() => handleDeleteContact(record.id)}
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
              onClick={() => handleSetDefaultContact(record.id)}
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
          setEditingContact(null);
          contactForm.resetFields();
          setContactModalVisible(true);
        }}
        style={{ marginBottom: 16 }}
      >
        Yeni İletişim Bilgisi
      </Button>

      <Table
        columns={contactColumns}
        dataSource={contacts}
        rowKey="id"
        loading={contactLoading}
      />

      <Modal
        title={editingContact ? 'İletişim Bilgisini Düzenle' : 'Yeni İletişim Bilgisi Ekle'}
        open={contactModalVisible}
        onCancel={() => {
          setContactModalVisible(false);
          setEditingContact(null);
          contactForm.resetFields();
        }}
        footer={null}
      >
        <Form
          form={contactForm}
          onFinish={editingContact ? handleUpdateContact : handleAddContact}
          layout="vertical"
        >
          <Form.Item
            name="contactType"
            label="İletişim Tipi"
            rules={[{ required: true, message: 'Lütfen iletişim tipi seçin' }]}
          >
            <Select>
              <Select.Option value="Email">E-posta</Select.Option>
              <Select.Option value="Phone">Telefon</Select.Option>
              <Select.Option value="Mobile">Cep Telefonu</Select.Option>
              <Select.Option value="Work">İş Telefonu</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            name="contactName"
            label="İletişim Adı"
            rules={[{ required: true, message: 'Lütfen iletişim adı girin' }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="contactValue"
            label="İletişim Değeri"
            rules={[{ required: true, message: 'Lütfen iletişim değeri girin' }]}
          >
            <Input />
          </Form.Item>

          <Form.Item>
            <Space>
              <Button type="primary" htmlType="submit" loading={contactLoading}>
                {editingContact ? 'Güncelle' : 'Ekle'}
              </Button>
              <Button onClick={() => {
                setContactModalVisible(false);
                setEditingContact(null);
                contactForm.resetFields();
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

export default ContactTab; 