'use client';
import { ProfileHeader } from "../Components/Navigation/ProfileHeader";
import styles from  "./layout.module.css";

export default function ProfileLayout({
  children,
}: {
  children: React.ReactNode;
}) {

  return (
    <div className={styles.container}>
      <ProfileHeader/>
      <main className={styles.content}>
        {children}
      </main>
    </div>
  );
}