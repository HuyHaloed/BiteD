import {
  Card,
  CardContent,
  Typography,
  List,
  ListItem,
  ListItemText,
} from "@mui/material";

function HowToCancel() {
  return (
    <Card
      style={{
        marginTop: "16px",
        // padding: "16px",
        backgroundColor: "#f5f5f5",
        border: "1px dashed #ccc",
      }}
    >
      <CardContent>
        <Typography variant="h6" gutterBottom fontWeight={600}>
          How To Cancel?
        </Typography>
        <List sx={{ listStyleType: "disc", marginX: 2 }}>
          <ListItem sx={{ display: "list-item" }}>
            <ListItemText primary="You can cancel your order before 4PM the day before" />
          </ListItem>
          <ListItem sx={{ display: "list-item" }}>
            <ListItemText primary="Once cancelled, you can have a chance to re-order another dish" />
          </ListItem>
          <ListItem sx={{ display: "list-item" }}>
            <ListItemText primary="You can re-order before 4PM the day before" />
          </ListItem>
        </List>
      </CardContent>
    </Card>
  );
}

export default HowToCancel;
