import { useParams } from "react-router-dom";
import LocationForm from "../features/super-admin/LocationForm";
import $api from "../services/api";
import ErrorMessage from "../ui/ErrorMessage";
import FullscreenLoading from "../ui/FullscreenLoading";

function Location() {
  const { locationId } = useParams() as { locationId: string };
  const location = $api.useQuery("get", "/swagger/Locations/{id}", {
    params: { path: { id: parseInt(locationId) } },
  });

  if (location.isLoading) return <FullscreenLoading />;
  if (location.error) return <ErrorMessage error={location.error} />;

  return (
    <>
      {/* <Debug obj={location.data}>Location</Debug> */}
      <LocationForm location={location.data}></LocationForm>
    </>
  );
}

export default Location;
