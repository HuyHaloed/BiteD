import { Button, Chip, Stack, Typography } from "@mui/material";
import $api from "../../services/api";
import { components } from "../../services/api.openapi";
import { getDailyOrderStatus } from "../../utils";
import { Delete } from "@mui/icons-material";
import HowToCancel from "./HowToCancel";
import ShiftOrderInfo from "./ShiftOrderInfo";
import { useEffect } from "react";

interface DailyOrderedInfoProps {
  orderInfo: components["schemas"]["DailyOrderDto"];
}

function DailyOrderedInfo({ orderInfo }: DailyOrderedInfoProps) {
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  const cancelOrder = $api.useMutation("post", "/swagger/Orders/{id}/cancel");

  return (
    <>
      <Stack spacing={2} alignItems="center" mb={2}>
        <Typography variant="h6" textAlign="center">
          You have successfully ordered your meal 🎉
        </Typography>
        <Chip label={getDailyOrderStatus(orderInfo.status)} color="primary" />
      </Stack>
      <Stack spacing={2}>
        {orderInfo.shiftOrders.map((shiftOrder) => (
          <ShiftOrderInfo key={shiftOrder.shiftType} shiftOrder={shiftOrder} />
        ))}
      </Stack>

      <HowToCancel />
      <Button
        variant="contained"
        color="primary"
        onClick={() =>
          cancelOrder.mutate({
            params: { path: { id: orderInfo.id } },
          })
        }
        disabled={cancelOrder.isPending || !orderInfo.canCancelOrder}
        style={{ position: "fixed", bottom: "80px", right: "16px" }}
        endIcon={<Delete />}
      >
        Cancel order
      </Button>
    </>
  );
}

export default DailyOrderedInfo;
