import React from "react";
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
} from "@mui/material";
import { components, DishType, ShiftType } from "../../services/api.openapi";
import { getEnumValues } from "../../utils";

const MonthlyMenuTable: React.FC<{
  data: components["schemas"]["MonthlyMenuDto"];
}> = ({ data }) => {
  return (
    <TableContainer component={Paper}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Date</TableCell>
            <TableCell>Shift</TableCell>
            {getEnumValues(DishType).map(({ value }) => (
              <TableCell key={value}>{value}</TableCell>
            ))}
          </TableRow>
        </TableHead>
        <TableBody>
          {data.dailyMenus.map((dailyMenu) =>
            dailyMenu.shiftMenus.map((shiftMenu, index) => (
              <TableRow key={`${dailyMenu.date}-${shiftMenu.shift}`}>
                {index === 0 && (
                  <TableCell rowSpan={dailyMenu.shiftMenus.length}>
                    {dailyMenu.date}
                  </TableCell>
                )}
                <TableCell>{ShiftType[shiftMenu.shift]}</TableCell>
                {getEnumValues(DishType).map(({ key }) => (
                  <TableCell key={key}>
                    {shiftMenu.dishes
                      .filter((dish) => dish.type.toString() === key)
                      .map((dish) => dish.name)
                      .join(", ")}
                  </TableCell>
                ))}
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default MonthlyMenuTable;
