import { Inter } from "next/font/google";
import "./globals.css";
import Header from "./Components/Navigation/Header";
import { UserProvider } from "./Contexts/UserContext";

const inter = Inter({ subsets: ["latin"] });

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html>
      <body className={inter.className}>
        <UserProvider>
          <Header/>
          {children}
        </UserProvider>
      </body>
    </html>
  );
}
