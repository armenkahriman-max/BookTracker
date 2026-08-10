import { Route, Routes } from "react-router-dom";
import { AccountPage } from "./auth/AccountPage";
import { LoginPage } from "./auth/LoginPage";
import { BookListPage } from "./books/BookListPage";
import { BookDetailsPage } from "./books/BookDetailsPage";
import { RequireAdministrator } from "./auth/RequireAdministrator";
import { CreateBookPage } from "./books/CreateBookPage";
import { EditBookPage } from "./books/EditBookPage";
import { RegisterPage } from "./members/RegisterPage";
import { Navigation } from "./navigation";
import { EditAccountPage } from "./auth/EditAccountPage";
import { MemberListPage } from "./members/MemberListPage";
import { MemberDetailsPage } from "./members/MemberDetailsPage";
import { EditMemberPage } from "./members/EditMemberPage";

function HomePage() {
  return <h1>Book Tracker</h1>;
}

export default function App() {
  return (
    <>
      <Navigation />

      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/account" element={<AccountPage />} />
        <Route path="/account/edit" element={<EditAccountPage />} />  
        <Route path="/books" element={<BookListPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/books/:bookId" element={<BookDetailsPage />} />

         {/*Admin*/}
        <Route element={<RequireAdministrator />}>
          <Route path="/books/new" element={<CreateBookPage />} />
          <Route path="/books/:bookId/edit" element={<EditBookPage />} />
          <Route path="/members/:memberId/edit" element={<EditMemberPage />} />
          {/*Member */}
          <Route path="/members" element={<MemberListPage />} />
    <Route path="/members/:memberId" element={<MemberDetailsPage />} />
        </Route>
      </Routes>
    </>
  );
}