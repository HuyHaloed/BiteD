import createFetchClient, { Middleware } from "openapi-fetch";
import createClient from "openapi-react-query";
import { paths } from "./api.openapi";
import { acquireToken } from "./auth";
import { toast } from "react-hot-toast";
import { Box, Button, Typography } from "@mui/material";
import {
  InfoOutlined,
  SentimentDissatisfiedOutlined,
} from "@mui/icons-material";

const fetchClient = createFetchClient<paths>({
  baseUrl: import.meta.env.VITE_API_BASE_URL,
});

const AuthMiddleware: Middleware = {
  async onRequest({ request }) {
    request.headers.set("Authorization", "Bearer " + (await acquireToken()));
    console.log(request)
    return request;
  },
};

const ErrorToastMiddleware: Middleware = {
  async onResponse({ response }) {
    if (!response.ok) {
      if (response.status === 500) {
        toast.error("Internal server error");
      } else {
        const error = await response.clone().json();
        console.log(error);
        switch (response.status) {
          case 400:
            if (error.title === "Invalid Operation") {
              toast.error(`Invalid operation: ${error.detail}`);
            } else {
              const errors = error.errors;
              for (const [attribute, messages] of Object.entries(errors)) {
                (messages as string[]).forEach((message) => {
                  toast.error(`${attribute}: ${message}`);
                });
              }
              toast(() => (
                <Box display="flex" alignItems="center" gap={1}>
                  <SentimentDissatisfiedOutlined color="error" />
                  <Typography variant="body1">
                    One or more errors occurred {error.detail}
                  </Typography>
                  <Button
                    variant="text"
                    size="small"
                    color="error"
                    startIcon={<InfoOutlined />}
                    onClick={() => alert(JSON.stringify(errors, null, 2))}
                  >
                    Details
                  </Button>
                </Box>
              ));
            }
            break;
          case 401:
            toast.error("Unauthorized");
            break;
          case 403:
            toast.error("Forbidden");
            break;
          default:
            toast.error(
              `Error: ${
                error.detail ??
                JSON.stringify(error.errors) ??
                "Something went wrong"
              }`
            );
            break;
        }
      }
    }
    return response;
  },
};

fetchClient.use(AuthMiddleware);
fetchClient.use(ErrorToastMiddleware);

const $api = createClient(fetchClient);
const $fetchClient = fetchClient;
export { $fetchClient };
export default $api;
