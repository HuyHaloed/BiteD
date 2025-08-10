import { useParams } from "react-router-dom";
import SupplierForm from "../features/super-admin/SupplierForm";
import $api from "../services/api";
import ErrorMessage from "../ui/ErrorMessage";
import FullscreenLoading from "../ui/FullscreenLoading";

export default function Supplier() {
  const { supplierId } = useParams() as { supplierId: string };
  const supplier = $api.useQuery("get", "/swagger/Suppliers/{id}", {
    params: { path: { id: parseInt(supplierId) } },
  });

  if (supplier.isLoading) return <FullscreenLoading />;
  if (supplier.error) return <ErrorMessage error={supplier.error} />;

  return (
    <>
      {/* <Debug obj={supplier.data}>Supplier</Debug> */}
      <SupplierForm supplier={supplier.data} />
    </>
  );
}
