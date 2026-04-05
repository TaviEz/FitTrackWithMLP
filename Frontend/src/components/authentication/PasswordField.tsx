import { Visibility, VisibilityOff } from "@mui/icons-material";
import { IconButton, InputAdornment, TextField } from "@mui/material";

const PasswordField = ({
  label,
  value,
  onChange,
  error,
  helperText,
  show,
  toggleShow
}: {
  label: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  error: boolean;
  helperText: string;
  show: boolean;
  toggleShow: () => void;
}) => (
  <TextField
    label={label}
    variant="standard"
    required
    fullWidth
    type={show ? "text" : "password"}
    value={value}
    onChange={onChange}
    error={error}
    helperText={helperText}
    InputProps={{
      endAdornment: (
        <InputAdornment position="end">
          <IconButton
            onClick={toggleShow}
            onMouseDown={(e) => e.preventDefault()}
            onMouseUp={(e) => e.preventDefault()}
          >
            {show ? <VisibilityOff /> : <Visibility />}
          </IconButton>
        </InputAdornment>
      )
    }}
  />
);

export default PasswordField;
