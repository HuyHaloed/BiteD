import {
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Stack,
  Button,
} from "@mui/material";
import { ReactNode } from "react";

function Debug({ obj, children }: { obj: unknown; children: ReactNode }) {
  return (
    <Stack direction="row" spacing={2} marginY={2}>
      <Accordion>
        <AccordionSummary
          // expandIcon={<ExpandMoreIcon />}
          aria-controls="panel1-content"
          id="panel1-header"
        >
          Debug {children}
        </AccordionSummary>
        <AccordionDetails>
          <pre>{JSON.stringify(obj, null, 2)}</pre>
        </AccordionDetails>
      </Accordion>
      <Button variant="text" onClick={() => console.log(obj)}>
        Print
      </Button>
    </Stack>
  );
}

export default Debug;
