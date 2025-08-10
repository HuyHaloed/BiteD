import React, { useState, useCallback } from "react";
import { List, Stack, Typography, Button } from "@mui/material";
import MealOptionsStack, { MealOption } from "./MealOptionStack";
import {
  components,
  LocationType,
  DishType,
  ShiftType,
} from "../../services/api.openapi";
import $api from "../../services/api";
import { getShiftType } from "../../utils";
import { Send } from "@mui/icons-material";
import toast from "react-hot-toast";
import FullscreenLoading from "../../ui/FullscreenLoading";

interface DailyOrderProps {
  menu: components["schemas"]["DailyMenuDto"];
  location: components["schemas"]["LocationDto"];
  canOrder: boolean;
  onOrderSuccess: () => void;
}

const DailyOrder: React.FC<DailyOrderProps> = ({
  menu,
  location,
  canOrder,
  onOrderSuccess,
}) => {
  const [selectedDishes, setSelectedDishes] = useState<{
    [shift: number]: number[];
  }>({});
  const createOrder = $api.useMutation("post", "/swagger/Orders", {
    onSuccess: () => {
      toast.success("Order created 🎉");
      onOrderSuccess();
    },
  });

  const getMealOptions = (
    shiftMenu: components["schemas"]["ShiftMenuDto"]
  ): MealOption[] => {
    const shiftDishes = shiftMenu.dishes;

    if (location.type === LocationType.Brewery) {
      if (
        (location.enableShift1 && shiftMenu.shift === ShiftType.Shift1) ||
        (location.enableShift2 && shiftMenu.shift === ShiftType.Shift2)
      ) {
        return [
          {
            id: 1,
            name: "Main dishes",
            maxSelection: 2,
            singleSelection: false,
            dishes: shiftDishes.filter((dish) => dish.type === DishType.Main),
          },
          {
            id: 2,
            name: "Soup",
            maxSelection: 1,
            singleSelection: true,
            dishes: shiftDishes.filter((dish) => dish.type === DishType.Soup),
          },
          {
            id: 3,
            name: "Vegetarian combo",
            maxSelection: 1,
            singleSelection: true,
            dishes: shiftDishes.filter(
              (dish) => dish.type === DishType.Vegetarian
            ),
          },
        ];
      } else if (
        location.enableShift3 &&
        shiftMenu.shift === ShiftType.Shift3
      ) {
        return [
          {
            id: 4,
            name: "Main dishes",
            maxSelection: 1,
            singleSelection: true,
            dishes: shiftDishes.filter((dish) => dish.type === DishType.Main),
          },
        ];
      }
    } else if (
      location.type === LocationType.HeadOffice &&
      shiftMenu.shift === ShiftType.Shift2
    ) {
      return [
        {
          id: 5,
          name: "Main dishes",
          maxSelection: 1,
          singleSelection: true,
          dishes: shiftDishes.filter((dish) => dish.type === DishType.Main),
        },
        {
          id: 6,
          name: "Vegetarian combo",
          maxSelection: 1,
          singleSelection: true,
          dishes: shiftDishes.filter(
            (dish) => dish.type === DishType.Vegetarian
          ),
        },
      ];
    }
    return [];
  };

  const handleMealSelect = useCallback(
    (shift: number) => (dishIds: number[]) => {
      setSelectedDishes((prev) => {
        const updatedDishes = { ...prev };
        if (dishIds.length === 0) {
          delete updatedDishes[shift];
        } else {
          updatedDishes[shift] = dishIds;
        }
        return updatedDishes;
      });
    },
    []
  );

  function handleOrder() {
    console.log(selectedDishes);
    const shiftOrders = Object.entries(selectedDishes).map(
      ([shift, dishIds]) => ({
        shiftType: parseInt(shift),
        dishIds,
      })
    );

    const orderData = {
      locationId: location.id,
      monthlyMenuId: menu.monthlyMenuId,
      date: menu.date,
      shiftOrders,
    };

    createOrder.mutate({ body: orderData });
  }

  return (
    <>
      {/* <Debug obj={selectedDishes}>Selected dishes</Debug> */}
      {createOrder.isPending && <FullscreenLoading />}
      <List>
        {menu.shiftMenus.map((shiftMenu, shiftIndex) => (
          <Stack direction="column" key={shiftIndex + menu.date} mb={3}>
            <Typography variant="body1" fontWeight={600}>
              {getShiftType(shiftMenu.shift).toUpperCase()}
            </Typography>
            <MealOptionsStack
              options={getMealOptions(shiftMenu)}
              onMealSelect={handleMealSelect(shiftMenu.shift)}
            />
          </Stack>
        ))}
      </List>
      <Button
        variant="contained"
        color="primary"
        onClick={() => handleOrder()}
        disabled={createOrder.isPending || !canOrder}
        style={{ position: "fixed", bottom: "80px", right: "16px" }}
        endIcon={<Send />}
      >
        Order Meal
      </Button>
    </>
  );
};

export default DailyOrder;
