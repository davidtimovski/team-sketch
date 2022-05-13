CREATE TABLE public.rooms
(
    id serial NOT NULL,
    name character varying(7) NOT NULL COLLATE pg_catalog."default",
	is_public boolean NOT NULL,
    created timestamp with time zone NOT NULL,
	CONSTRAINT "PK_rooms" PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.rooms
    OWNER to postgres;
	