import { Select } from 'antd';
import { Gender } from '../../types/Enums';

const { Option } = Select;

export const GENDER_OPTIONS = [
    { value: Gender.Unspecified, label: 'Belirtilmemiş' },
    { value: Gender.Male, label: 'Erkek' },
    { value: Gender.Female, label: 'Kadın' }
] as const;

interface GenderSelectProps {
    value?: Gender;
    onChange?: (value: Gender) => void;
    disabled?: boolean;
}

const GenderSelect = ({ value, onChange, disabled }: GenderSelectProps) => {
    return (
        <Select
            value={value}
            onChange={onChange}
            disabled={disabled}
            placeholder="Cinsiyet seçiniz"
        >
            {GENDER_OPTIONS.map(option => (
                <Option key={option.value} value={option.value}>
                    {option.label}
                </Option>
            ))}
        </Select>
    );
};

export default GenderSelect; 