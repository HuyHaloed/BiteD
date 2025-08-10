import {
  AccessTime,
  FastfoodOutlined,
  LabelOutlined,
} from "@mui/icons-material";
import { Card, CardContent, Stack, Typography } from "@mui/material";
import { getShiftType, getShiftOrderStatus } from "../../utils";
import { components } from "../../services/api.openapi";

export default function ShiftOrderInfo({
  shiftOrder,
}: {
  shiftOrder: components["schemas"]["ShiftOrderDto"];
}) {
  return (
    <Card
      key={shiftOrder.shiftType}
      sx={{
        backgroundColor: "secondary.light",
        borderRadius: "16px",
        border: "1px solid",
        borderColor: "primary.light",
        elevation: 2,
      }}
    >
      <CardContent>
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="center"
        >
          <Stack direction="row" alignItems="center">
            <AccessTime />{" "}
            <Typography
              variant="h6"
              style={{ marginLeft: "8px", fontWeight: 600 }}
            >
              {getShiftType(shiftOrder.shiftType)}
            </Typography>
          </Stack>
          <Stack direction="row" alignItems="center">
            <LabelOutlined />{" "}
            <Typography
              variant="h6"
              style={{ marginLeft: "8px", fontWeight: 600 }}
            >
              {getShiftOrderStatus(shiftOrder.status)}
            </Typography>
          </Stack>
        </Stack>
        <Stack direction="row" alignItems="center" mb={1} mt={2}>
          <FastfoodOutlined />{" "}
          <Typography
            variant="body1"
            style={{ marginLeft: "8px", fontWeight: 600 }}
          >
            Dishes
          </Typography>
        </Stack>
        <Stack direction="column">
          {shiftOrder.dishes.map((dish) => (
            <Typography variant="body2" key={dish.id}>
              {dish.name}
            </Typography>
          ))}
        </Stack>
      </CardContent>
    </Card>
  );
}
