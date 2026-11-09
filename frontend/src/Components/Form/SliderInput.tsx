import classNames from 'classnames';
import React, { ReactNode, useCallback } from 'react';
import { InputChanged } from 'typings/inputs';
import styles from './SliderInput.module.css';

export interface SliderInputProps {
  className?: string;
  name: string;
  value?: number | null;
  min: number;
  max: number;
  step?: number;
  hasError?: boolean;
  hasWarning?: boolean;
  isDisabled?: boolean;
  formatValue?: (value: number) => ReactNode;
  onChange: (change: InputChanged<number>) => void;
}

function SliderInput({
  className = styles.slider,
  name,
  value = null,
  min,
  max,
  step = 1,
  hasError = false,
  hasWarning = false,
  isDisabled = false,
  formatValue,
  onChange,
}: SliderInputProps) {
  const currentValue = value ?? min;

  const handleChange = useCallback(
    (event: React.ChangeEvent<HTMLInputElement>) => {
      onChange({
        name,
        value: parseInt(event.target.value, 10),
      });
    },
    [name, onChange]
  );

  return (
    <div className={styles.container}>
      <input
        className={classNames(
          className,
          hasError && styles.hasError,
          hasWarning && styles.hasWarning
        )}
        type="range"
        name={name}
        value={currentValue}
        min={min}
        max={max}
        step={step}
        disabled={isDisabled}
        onChange={handleChange}
      />

      <div className={styles.value}>
        {formatValue ? formatValue(currentValue) : currentValue}
      </div>
    </div>
  );
}

export default SliderInput;
