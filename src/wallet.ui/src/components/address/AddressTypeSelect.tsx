import { Select } from 'antd';
import { AddressType } from '../../types/enums';

const { Option } = Select;

export const ADDRESS_TYPE_OPTIONS = [
    { value: AddressType.Home, label: 'Ev' },
    { value: AddressType.Work, label: 'İş' },
    { value: AddressType.Other, label: 'Diğer' }
] as const;

interface AddressTypeSelectProps {
    value?: AddressType;
    onChange?: (value: AddressType) => void;
    disabled?: boolean;
}

const AddressTypeSelect = ({ value, onChange, disabled }: AddressTypeSelectProps) => {
    return (
        <Select
            value={value}
            onChange={onChange}
            disabled={disabled}
            placeholder="Adres tipi seçiniz"
        >
            {ADDRESS_TYPE_OPTIONS.map(option => (
                <Option key={option.value} value={option.value}>
                    {option.label}
                </Option>
            ))}
        </Select>
    );
};

export default AddressTypeSelect; 