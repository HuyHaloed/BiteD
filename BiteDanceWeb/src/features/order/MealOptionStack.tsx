import React, { useState } from "react";
import MealOptionCard from "./MealOptionCard";
import { Stack } from "@mui/material";

interface Dish {
  id: number;
  name: string;
}

export interface MealOption {
  id: number;
  name: string;
  maxSelection: number;
  singleSelection: boolean;
  dishes: Dish[];
}

interface MealOptionsStackProps {
  options: MealOption[];
  onMealSelect: (selectedMeals: number[]) => void;
}

const MealOptionsStack: React.FC<MealOptionsStackProps> = ({
  options,
  onMealSelect,
}) => {
  const [selectedOption, setSelectedOption] = useState<number | null>(null);
  const [selectedMeals, setSelectedMeals] = useState<number[]>([]);

  const handleMealSelect = (option: MealOption, mealId: number) => {
    if (selectedOption !== option.id) {
      // Reset the selected meals when switching options
      setSelectedOption(option.id);
      const newSelectedMeals = [mealId];
      setSelectedMeals(newSelectedMeals);
      onMealSelect(newSelectedMeals);

      return;
    }

    setSelectedMeals((prevSelectedMeals) => {
      let newSelectedMeals;
      if (option.singleSelection) {
        // Single selection logic
        newSelectedMeals = [mealId];
      } else {
        const isSelected = prevSelectedMeals.includes(mealId);
        if (isSelected) {
          // Deselect the meal
          newSelectedMeals = prevSelectedMeals.filter((id) => id !== mealId);
        } else if (prevSelectedMeals.length < option.maxSelection) {
          // Add the meal if not at max selection
          newSelectedMeals = [...prevSelectedMeals, mealId];
        } else {
          // Replace the oldest meal with the new meal
          newSelectedMeals = [...prevSelectedMeals.slice(1), mealId];
        }
      }
      onMealSelect(newSelectedMeals);

      return newSelectedMeals;
    });
  };

  const handleOptionSelect = (option: MealOption) => {
    // Selected itself -> deselect
    if (selectedOption === option.id) {
      setSelectedOption(null);
      setSelectedMeals([]);
      onMealSelect([]);
      return;
    }

    setSelectedOption(option.id);
    const initialMeals = option.singleSelection
      ? [option.dishes[0].id] // Single selection logic
      : option.dishes.slice(0, option.maxSelection).map((dish) => dish.id); // Multiple selection logic
    setSelectedMeals(initialMeals);
    onMealSelect(initialMeals);
  };

  return (
    <Stack
      direction="row"
      spacing={2}
      // marginY={2}
      py={2}
      style={{ overflowX: "auto", userSelect: "none" }}
    >
      {options.map((option) => (
        <MealOptionCard
          key={option.id}
          option={option}
          selected={selectedOption === option.id}
          selectedMeals={selectedOption === option.id ? selectedMeals : []}
          handleMealSelect={(mealId: number) =>
            handleMealSelect(option, mealId)
          }
          handleOptionSelect={() => handleOptionSelect(option)}
        />
      ))}
    </Stack>
  );
};

export default MealOptionsStack;
