'use client';

import React, { useState } from 'react';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { ReactProps } from '../types';
import { DndContext, DragOverlay } from '@dnd-kit/core';

export function Draggable({ id, children }: ReactProps & { id: string }) {
  const [activeId, setActiveId] = useState<string | null>(null);

  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    zIndex: isDragging ? 999 : 'auto',
    boxShadow: isDragging ? '0 4px 8px rgba(0, 0, 0, 0.2)' : 'none',
    transform: isDragging ? 'scale(1.1)' : CSS.Transform.toString(transform),
  };

  return (
    <DndContext
      onDragStart={({ active }) => setActiveId(active.id)}
      onDragEnd={() => setActiveId(null)}
    >
      <div ref={setNodeRef} style={style} {...attributes} {...listeners}>
        {children}
      </div>
      <DragOverlay>
        {activeId ? <div>{children}</div> : null}
      </DragOverlay>
    </DndContext>
  );
}
