import React, { useState } from "react";
import {
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Button,
  TextField,
} from "@mui/material";

interface RedCodeDialogProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (note: string) => void;
  title: string;
  description: string;
}

const RedCodeDialog: React.FC<RedCodeDialogProps> = ({
  open,
  onClose,
  onSubmit,
  title,
  description,
}) => {
  const [note, setNote] = useState("");

  const handleSubmit = () => {
    onSubmit(note);
    setNote("");
  };

  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>{title}</DialogTitle>
      <DialogContent>
        <DialogContentText>{description}</DialogContentText>
        <TextField
          autoFocus
          margin="dense"
          label="Note"
          type="text"
          fullWidth
          value={note}
          onChange={(e) => setNote(e.target.value)}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button onClick={handleSubmit}>Submit</Button>
      </DialogActions>
    </Dialog>
  );
};

export default RedCodeDialog;
