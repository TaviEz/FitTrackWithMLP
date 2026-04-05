import { NumberField } from "@base-ui-components/react/number-field"
import React from "react";
import styles from "./InputNumber.module.css"
import theme from "../theme";
import type UserDetails from "../models/UserDetails";
import type { NumericUserDetailsField } from "../utils/types";

interface InputNumberProps {
  ariaLabel: string,
  label: string,
  field: NumericUserDetailsField;
  userDetails: UserDetails,
  setUserDetails: (userDetails: UserDetails) => void;
  maxValue: number;
}

export default function InputNumber ({ariaLabel, label, field, userDetails, setUserDetails, maxValue}: InputNumberProps) {
    const id = React.useId();
    
    const onChange = (newValue: number) => {
      if (typeof newValue === "number" && ["age", "weight", "height"].includes(field)) {
        const updated = userDetails.clone();
        updated[field as "age" | "weight" | "height"] = newValue;
        setUserDetails(updated);
      }
    };


    
    return (
        <NumberField.Root 
          id={id} 
          aria-label={ariaLabel} 
          className={styles.Field}
          value={userDetails[field]}
          max={maxValue}
          onValueChange={(value) => {
            if (value !== null) onChange(value)
          }}
        >
          <label htmlFor={id} className={styles.Label} style={theme.typography.body1}>
            {label}
          </label>
          <NumberField.Group className={styles.Group}>
            <NumberField.Input className={styles.Input} />
            <NumberField.Decrement className={styles.Decrement}>
              <MinusIcon />
            </NumberField.Decrement>
            <NumberField.Increment className={styles.Increment}>
              <PlusIcon />
            </NumberField.Increment>
          </NumberField.Group>

          <NumberField.ScrubArea className={styles.ScrubArea}>
            <NumberField.ScrubAreaCursor className={styles.ScrubAreaCursor}>
              <CursorGrowIcon />
            </NumberField.ScrubAreaCursor>
          </NumberField.ScrubArea>
      </NumberField.Root>
    )
}

function CursorGrowIcon(props: React.ComponentProps<'svg'>) {
  return (
    <svg
      width="26"
      height="14"
      viewBox="0 0 24 14"
      fill="black"
      stroke="white"
      xmlns="http://www.w3.org/2000/svg"
      {...props}
    >
      <path d="M19.5 5.5L6.49737 5.51844V2L1 6.9999L6.5 12L6.49737 8.5L19.5 8.5V12L25 6.9999L19.5 2V5.5Z" />
    </svg>
  );
}

function PlusIcon(props: React.ComponentProps<'svg'>) {
  return (
    <svg
      width="10"
      height="10"
      viewBox="0 0 10 10"
      fill="none"
      stroke="currentcolor"
      strokeWidth="1.6"
      xmlns="http://www.w3.org/2000/svg"
      {...props}
    >
      <path d="M0 5H5M10 5H5M5 5V0M5 5V10" />
    </svg>
  );
}

function MinusIcon(props: React.ComponentProps<'svg'>) {
  return (
    <svg
      width="10"
      height="10"
      viewBox="0 0 10 10"
      fill="none"
      stroke="currentcolor"
      strokeWidth="1.6"
      xmlns="http://www.w3.org/2000/svg"
      {...props}
    >
      <path d="M0 5H10" />
    </svg>
  );
}