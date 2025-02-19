import React, { useEffect, useState } from 'react';
import { Card, Row, Col, Avatar, Statistic, List, Tag, Typography, Skeleton } from 'antd';
import { Link } from 'react-router-dom';
import { UserOutlined, MessageOutlined, MailOutlined, PhoneOutlined, HomeOutlined } from '@ant-design/icons';
import { useSelector } from 'react-redux';
import { RootState } from '../store';
import { personService } from '../services/personService';
import { messageService } from '../services/messageService';
import { PersonData } from '../types/person';
import { IMessage } from '../types/message';
import dayjs from 'dayjs';

const { Title, Text } = Typography;

const Dashboard = () => {
    const { user } = useSelector((state: RootState) => state.auth);
    const [loading, setLoading] = useState(true);
    const [personData, setPersonData] = useState<PersonData | null>(null);
    const [messages, setMessages] = useState<IMessage[]>([]);

    useEffect(() => {
        fetchData();
    }, [user?.id]);

    const fetchData = async () => {
        try {
            setLoading(true);
            if (user?.id) {
                const [personResponse, messagesResponse] = await Promise.all([
                    personService.getPerson(user.id),
                    messageService.getInboxMessages().then(messages => messages.slice(0, 2))
                ]);
                setPersonData(personResponse);
                setMessages(messagesResponse);
            }
        } catch (error) {
            console.error('Error fetching dashboard data:', error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="dashboard">
            <Row gutter={[16, 16]}>
                {/* Profil Özeti */}
                <Col xs={24} lg={12}>
                    <Card title="Profil Bilgileri" extra={<Link to="/profile">Düzenle</Link>}>
                        <Skeleton loading={loading} avatar active>
                            <div className="profile-summary">
                                <div className="profile-header" style={{ display: 'flex', alignItems: 'center', marginBottom: 16 }}>
                                    <Avatar size={64} icon={<UserOutlined />} src={personData?.profilePictureUrl} />
                                    <div style={{ marginLeft: 16 }}>
                                        <Title level={4} style={{ margin: 0 }}>
                                            {personData?.firstName} {personData?.lastName}
                                        </Title>
                                        <Text type="secondary">{user?.email}</Text>
                                    </div>
                                </div>

                                <List size="small">
                                    {personData?.contacts?.slice(0, 2).map((contact, index) => (
                                        <List.Item key={index}>
                                            {contact.contactType === 'Email' ? (
                                                <MailOutlined style={{ marginRight: 8 }} />
                                            ) : (
                                                <PhoneOutlined style={{ marginRight: 8 }} />
                                            )}
                                            {contact.contactValue}
                                            {contact.isPrimary && <Tag color="blue" style={{ marginLeft: 8 }}>Birincil</Tag>}
                                        </List.Item>
                                    ))}
                                    {personData?.addresses?.slice(0, 1).map((address, index) => (
                                        <List.Item key={`address-${index}`}>
                                            <HomeOutlined style={{ marginRight: 8 }} />
                                            {address.addressName}
                                            {address.isDefault && <Tag color="blue" style={{ marginLeft: 8 }}>Varsayılan</Tag>}
                                        </List.Item>
                                    ))}
                                </List>
                            </div>
                        </Skeleton>
                    </Card>
                </Col>

                {/* Mesaj Özeti */}
                <Col xs={24} lg={12}>
                    <Card 
                        title={`Son Mesajlar ${messages.length}`} 
                        extra={<Link to="/messages">Tümünü Gör</Link>}
                    >
                        <Skeleton loading={loading} active>
                            <List
                                itemLayout="horizontal"
                                dataSource={messages}
                                renderItem={message => (
                                    <List.Item>
                                        <List.Item.Meta
                                            avatar={<Avatar icon={<MessageOutlined />} />}
                                            title={message.subject}
                                            description={
                                                <div>
                                                    <Text type="secondary" style={{ marginRight: 16 }}>
                                                        {dayjs(message.createdDate).format('DD.MM.YYYY HH:mm')}
                                                    </Text>
                                                    <Text ellipsis>{message.content}</Text>
                                                </div>
                                            }
                                        />
                                    </List.Item>
                                )}
                            />
                        </Skeleton>
                    </Card>
                </Col>

                {/* İstatistikler */}
                <Col xs={24}>
                    <Card>
                        <Row gutter={16}>
                            <Col span={6}>
                                <Statistic 
                                    title="Toplam Mesaj" 
                                    value={messages.length} 
                                    prefix={<MessageOutlined />} 
                                />
                            </Col>
                            <Col span={6}>
                                <Statistic 
                                    title="Adres Sayısı" 
                                    value={personData?.addresses?.length || 0}
                                    prefix={<HomeOutlined />}
                                />
                            </Col>
                            {/* Diğer istatistikler eklenebilir */}
                        </Row>
                    </Card>
                </Col>
            </Row>
        </div>
    );
};

export default Dashboard; 