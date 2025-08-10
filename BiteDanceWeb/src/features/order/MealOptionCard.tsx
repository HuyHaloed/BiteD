import React from "react";
import { Card, CardContent, Typography, Chip, Stack } from "@mui/material";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";

interface Dish {
  id: number;
  name: string;
}

interface MealOption {
  id: number;
  name: string;
  maxSelection: number;
  singleSelection: boolean;
  dishes: Dish[];
}

interface MealOptionCardProps {
  option: MealOption;
  selected: boolean;
  selectedMeals: number[];
  handleMealSelect: (mealId: number) => void;
  handleOptionSelect: () => void;
}

const MealOptionCard: React.FC<MealOptionCardProps> = ({
  option,
  selected,
  selectedMeals,
  handleMealSelect,
  handleOptionSelect,
}) => {
  const isSelected = (mealId: number) => selectedMeals.includes(mealId);

  return (
    <Card
      variant="outlined"
      sx={{
        minWidth: 275,
        margin: 2,
        borderColor: selected ? "green" : "grey",
        boxShadow: "2",
      }}
      onClick={handleOptionSelect}
    >
      <CardContent sx={{ padding: 0 }}>
        <Typography
          variant="h6"
          component="div"
          sx={{ backgroundColor: "yellow", padding: 2 }}
        >
          {option.name} ({selectedMeals.length}/{option.maxSelection})
        </Typography>
        <Stack spacing={1} mt={2} px={2}>
          {option.dishes.map((dish) => (
            <Chip
              key={dish.id}
              label={dish.name}
              onClick={(e) => {
                // e.currentTarget.blur(); // Unfocus the chip
                e.stopPropagation(); // Prevent card click when chip is clicked
                handleMealSelect(dish.id);
              }}
              sx={{
                backgroundColor: "lightgreen",
                border: isSelected(dish.id) ? "1px solid green" : "",
              }}
              icon={
                isSelected(dish.id) ? (
                  <CheckCircleIcon color="success" />
                ) : undefined
              }
            />
          ))}
        </Stack>
      </CardContent>
    </Card>
  );
};

export default MealOptionCard;
